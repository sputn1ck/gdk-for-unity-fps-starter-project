#!/bin/bash
protoc ./proto/service.proto --go_out=*
exit 0
