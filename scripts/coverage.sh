#!/bin/bash
rm -rf BookingApi.Tests/TestResults
rm -rf coverage-report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator \
  -reports:"BookingApi.Tests/TestResults/**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:Html \
  -classfilters:"-*.Generated*;-*Migrations*;-*Designer*"
open coverage-report/index.html