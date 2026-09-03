#!/usr/bin/env bash
# Scans the solution's NuGet dependencies (including transitive packages) for known
# vulnerabilities, per the constitution's dependency-hygiene requirement.
set -euo pipefail

cd "$(dirname "$0")/.."

DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export DOTNET_ROOT
export PATH="$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"

dotnet list Taskify.slnx package --vulnerable --include-transitive
