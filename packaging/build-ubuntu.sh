#!/usr/bin/env bash
set -euo pipefail

# Genera paquetes para Ubuntu 24.04 (x64): carpeta publicada, .deb, AppImage y tar.gz.
# Uso: DOTNET=/ruta/a/dotnet ./packaging/build-ubuntu.sh

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT="$ROOT/artifacts"
RID="linux-x64"
VERSION="1.1.0"
APP_NAME="Git Proxy Manager"
APP_ID="git-proxy-manager"
DOTNET="${DOTNET:-dotnet}"

PUBLISH_DIR="$OUT/publish/$RID"
APPDIR="$OUT/appimage/AppDir"
DEB_ROOT="$OUT/deb/${APP_ID}_${VERSION}_amd64"
TOOLS="$OUT/tools"

log() { printf '\033[1;34m==>\033[0m %s\n' "$*"; }

rm -rf "$OUT/publish" "$OUT/appimage" "$OUT/deb"
mkdir -p "$PUBLISH_DIR" "$APPDIR/usr/bin" "$DEB_ROOT/DEBIAN" "$TOOLS"

log "Restaurando y publicando ($RID, self-contained)..."
"$DOTNET" restore "$ROOT/GitProxyManager.csproj"
"$DOTNET" publish "$ROOT/GitProxyManager.csproj" \
  -c Release -r "$RID" --self-contained true \
  -p:PublishSingleFile=false \
  -o "$PUBLISH_DIR"

# ---------------- tar.gz ----------------
log "Creando tar.gz..."
tar -czf "$OUT/${APP_ID}-${VERSION}-${RID}.tar.gz" \
  -C "$PUBLISH_DIR" .

# ---------------- AppDir / AppImage ----------------
log "Preparando AppDir..."
cp -a "$PUBLISH_DIR/." "$APPDIR/usr/bin/"
cp "$ROOT/Resources/Icons/app-icon.png" "$APPDIR/${APP_ID}.png"

cat > "$APPDIR/AppRun" <<'EOF'
#!/usr/bin/env bash
HERE="$(dirname "$(readlink -f "$0")")"
export DOTNET_ROOT="$HERE/usr/bin"
export DOTNET_BUNDLE_EXTRACT_BASE_DIR="${TMPDIR:-/tmp}/git-proxy-manager-appimage"
exec "$HERE/usr/bin/GitProxyManager" "$@"
EOF
chmod +x "$APPDIR/AppRun"

cat > "$APPDIR/${APP_ID}.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=$APP_NAME
Comment=Gestor de proxy para Git
Exec=AppRun
Icon=$APP_ID
Categories=Utility;Network;
Terminal=false
EOF
cp "$APPDIR/${APP_ID}.desktop" "$APPDIR/usr/share/applications/" 2>/dev/null || {
  mkdir -p "$APPDIR/usr/share/applications"
  cp "$APPDIR/${APP_ID}.desktop" "$APPDIR/usr/share/applications/"
}

APPIMAGETOOL="$TOOLS/appimagetool-x86_64.AppImage"
if [[ ! -x "$APPIMAGETOOL" ]]; then
  log "Descargando appimagetool..."
  curl -fL --retry 2 \
    -o "$APPIMAGETOOL" \
    https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage || true
  chmod +x "$APPIMAGETOOL" 2>/dev/null || true
fi

if [[ -x "$APPIMAGETOOL" && $(stat -c%s "$APPIMAGETOOL" 2>/dev/null || echo 0) -gt 1000000 ]]; then
  log "Generando AppImage..."
  "$APPIMAGETOOL" --appimage-extract-and-run "$APPDIR" "$OUT/${APP_ID}-${VERSION}-x86_64.AppImage" || \
    "$APPIMAGETOOL" "$APPDIR" "$OUT/${APP_ID}-${VERSION}-x86_64.AppImage" || \
    echo "No se pudo generar el AppImage (se omite)."
else
  echo "appimagetool no disponible; se omite el AppImage."
fi

# ---------------- .deb ----------------
log "Generando paquete .deb..."
INSTALL_DIR="$DEB_ROOT/opt/$APP_ID"
mkdir -p "$INSTALL_DIR" \
  "$DEB_ROOT/usr/share/applications" \
  "$DEB_ROOT/usr/share/icons/hicolor/256x256/apps" \
  "$DEB_ROOT/usr/bin"

cp -a "$PUBLISH_DIR/." "$INSTALL_DIR/"
chmod +x "$INSTALL_DIR/GitProxyManager"
cp "$ROOT/Resources/Icons/app-icon.png" "$DEB_ROOT/usr/share/icons/hicolor/256x256/apps/${APP_ID}.png"

cp "$APPDIR/${APP_ID}.desktop" "$DEB_ROOT/usr/share/applications/${APP_ID}.desktop"
sed -i 's|^Exec=.*|Exec=/usr/bin/git-proxy-manager|' "$DEB_ROOT/usr/share/applications/${APP_ID}.desktop"

cat > "$DEB_ROOT/usr/bin/git-proxy-manager" <<'EOF'
#!/usr/bin/env bash
export DOTNET_ROOT=/opt/$APP_ID
exec /opt/$APP_ID/GitProxyManager "$@"
EOF
chmod +x "$DEB_ROOT/usr/bin/git-proxy-manager"

INSTALLED_SIZE=$(du -sk "$DEB_ROOT" | cut -f1)
cat > "$DEB_ROOT/DEBIAN/control" <<EOF
Package: $APP_ID
Version: $VERSION
Section: utils
Priority: optional
Architecture: amd64
Installed-Size: $INSTALLED_SIZE
Maintainer: GitProxyManager <gitproxymanager@example.com>
Description: Gestor de proxy para Git
 Aplicacion de escritorio multiplataforma para activar o desactivar el proxy
 de Git y del sistema desde una interfaz grafica o el area de notificacion.
Depends: libc6, libgcc-s1, libstdc++6, libfontconfig1, libx11-6, libxcb1, libxrender1
EOF

dpkg-deb --build --root-owner-group "$DEB_ROOT" "$OUT/${APP_ID}_${VERSION}_amd64.deb"

log "Artefactos generados en $OUT:"
find "$OUT" -maxdepth 1 -type f -printf '  %f (%s bytes)\n' | sort
