#!/bin/bash

docker run \
       -v ./DailyWirePodcastProxy.ini:/app/DailyWirePodcastProxy.ini \
       -v ./data:/app/data/ \
       -p 9473:9473 \
       --rm -it ghcr.io/fpnewton/dailywirepodcastproxy:master