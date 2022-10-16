#!/bin/bash

ARCH="$1"
STARTUP_PROJ="DailyWirePodcastProxy"

_SCRIPT_PATH=$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )

pushd ${_SCRIPT_PATH} >/dev/null 2>&1
pushd .. >/dev/null 2>&1

export MSBUILDDISABLENODEREUSE=1

exec dotnet publish \
     --configuration Release \
     --self-contained true \
     --runtime ${ARCH} \
     --output ./publish/${ARCH} \
     ./src/${STARTUP_PROJ}/${STARTUP_PROJ}.csproj

popd
popd