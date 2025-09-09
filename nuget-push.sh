#!/usr/bin/env bash
set -euo pipefail

ARTIFACTS_DIR="./artifacts"
NUGET_API_KEY=""
NUGET_SOURCE="https://api.nuget.org/v3/index.json"

echo "📦 Cleaning old packages..."
rm -rf "$ARTIFACTS_DIR"
mkdir -p "$ARTIFACTS_DIR"

echo "⚙️ Building & packing project..."
dotnet pack -c Release -o "$ARTIFACTS_DIR"

echo "🚀 Pushing NuGet packages..."
for pkg in "$ARTIFACTS_DIR"/*.nupkg; do
  if [[ "$pkg" == *Sample* || "$pkg" == *DDL* ]]; then
    echo "⏭️ Skipping package: $(basename "$pkg")"
    continue
  fi

  echo "➡️ Pushing package: $(basename "$pkg")"
  dotnet nuget push "$pkg" \
    --api-key "$NUGET_API_KEY" \
    --source "$NUGET_SOURCE" \
    --skip-duplicate
done

echo "✅ Done."
