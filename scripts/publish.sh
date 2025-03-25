#!/usr/bin/env bash
set -e  # Exit immediately if any command fails

# Ensure we always return to the original directory when the script exits
trap 'popd 2>/dev/null || true' EXIT

_SOLUTION_FILE="./DailyWirePodcastProxy.sln" 
_WEB_CSPROJ="./src/PodcastProxy.Web/PodcastProxy.Web.csproj"
_OUTPUT_DIR="./publish"
_CONFIGURATION="Release"

_TARGETS=("linux-x64" "osx-arm64" "osx-x64" "win-x64")

# Get the script's directory
_SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Move to the parent directory of the script
pushd "$_SCRIPT_DIR/.." || { echo "Failed to change directory"; exit 1; }

# Ensure the output directory exists
mkdir -p "${_OUTPUT_DIR}"

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore "${_SOLUTION_FILE}"

# Build the solution (without publishing)
echo "Building solution..."
dotnet build "${_SOLUTION_FILE}" -c "${_CONFIGURATION}" --no-restore

# Remove previous builds and zips
pushd "${_OUTPUT_DIR}" || { echo "Failed to change directory to output dir"; exit 1; }

for target in "${_TARGETS[@]}"; do
  if [ -d "${target}" ]; then
    echo "Removing existing directory: ${target}"
    rm -rf "${target}"
  fi
  if [ -f "${target}.zip" ]; then
    echo "Removing existing zip: ${target}.zip"
    rm -f "${target}.zip"
  fi
done

popd  # Return from output dir

# Build and publish each target
for target in "${_TARGETS[@]}"; do
  echo "Publishing for ${target}..."
  dotnet publish \
    "${_WEB_CSPROJ}" \
    -c "${_CONFIGURATION}" \
    -r "${target}" \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:PublishReadyToRun=true \
    --output "${_OUTPUT_DIR}/${target}" \
    --force \
    --no-restore --no-build  # Avoid redundant restore/build
done

# Move back to the output directory for zipping
pushd "${_OUTPUT_DIR}" || { echo "Failed to change directory to output dir"; exit 1; }

# Zip each target folder without the top-level folder in the zip
for target in "${_TARGETS[@]}"; do
  if [ -d "${target}" ] && [ "$(ls -A "${target}")" ]; then
    echo "Creating zip for ${target}..."
    zip -r "${target}.zip" "${target}"/*
  fi
done

popd  # Return to the original directory
