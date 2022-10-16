#!/bin/bash

_SCRIPT_PATH=$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )

pushd ${_SCRIPT_PATH} >/dev/null 2>&1
pushd .. >/dev/null 2>&1

rm -rf ./publish

./build/build-arch.sh linux-x64
./build/build-arch.sh win-x64
./build/build-arch.sh osx-x64

cp ./build/dailywire-podcast-proxy.service ./publish/linux-x64/dailywire-podcast-proxy.service

popd
popd