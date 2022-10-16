#!/bin/bash

MIGRATION_PROJECT="src/Podcasts/Podcasts.csproj"
STARTUP_PROJECT="src/DailyWirePodcastProxy/DailyWirePodcastProxy.csproj"
DB_CONTEXT="PodcastDbContext"

_SCRIPT_PATH=$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )

pushd $_SCRIPT_PATH >/dev/null 2>&1
pushd .. >/dev/null 2>&1

exec dotnet ef "$@" -p ${MIGRATION_PROJECT} -s ${STARTUP_PROJECT} -c ${DB_CONTEXT}

popd
popd
