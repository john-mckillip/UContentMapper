#!/bin/bash

# UContentMapper Test Execution Script
# This script runs all tests with coverage reporting

set -e

echo "🧪 UContentMapper Test Suite"
echo "============================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Default values
CONFIGURATION="Release"
COVERAGE_THRESHOLD=90
VERBOSE=false
OPEN_REPORT=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        -t|--threshold)
            COVERAGE_THRESHOLD="$2"
            shift 2
            ;;
        -v|--verbose)
            VERBOSE=true
            shift
            ;;
        -o|--open-report)
            OPEN_REPORT=true
            shift
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  -c, --configuration    Build configuration (Debug|Release) [default: Release]"
            echo "  -t, --threshold       Coverage threshold percentage [default: 90]"
            echo "  -v, --verbose         Enable verbose output"
            echo "  -o, --open-report     Open coverage report in browser after completion"
            echo "  -h, --help            Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo -e "${BLUE}Configuration:${NC} $CONFIGURATION"
echo -e "${BLUE}Coverage Threshold:${NC} $COVERAGE_THRESHOLD%"
echo ""

# Clean previous test results
echo -e "${YELLOW}🧹 Cleaning previous test results...${NC}"
rm -rf TestResults/
mkdir -p TestResults

# Restore dependencies
echo -e "${YELLOW}📦 Restoring dependencies...${NC}"
dotnet restore

# Build solution
echo -e "${YELLOW}🔨 Building solution...${NC}"
dotnet build --no-restore --configuration $CONFIGURATION

# Run tests with coverage
echo -e "${YELLOW}🧪 Running tests with coverage...${NC}"

VERBOSITY_LEVEL="normal"
if [ "$VERBOSE" = true ]; then
    VERBOSITY_LEVEL="detailed"
fi

dotnet test --no-build --configuration $CONFIGURATION \
    --settings coverlet.runsettings \
    --collect:"XPlat Code Coverage" \
    --results-directory ./TestResults \
    --logger "trx;LogFileName=test-results.trx" \
    --logger "console;verbosity=$VERBOSITY_LEVEL" \
    --verbosity $VERBOSITY_LEVEL \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=cobertura,lcov,opencover \
    /p:CoverletOutput=./TestResults/coverage \
    /p:Threshold=$COVERAGE_THRESHOLD \
    /p:ThresholdType=line,branch,method \
    /p:ThresholdStat=total

TEST_EXIT_CODE=$?

# Generate coverage report if tests passed
if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo -e "${YELLOW}📊 Generating coverage report...${NC}"
    
    # Check if reportgenerator is installed globally
    if ! command -v reportgenerator &> /dev/null; then
        echo -e "${YELLOW}Installing ReportGenerator tool...${NC}"
        dotnet tool install -g dotnet-reportgenerator-globaltool
    fi
    
    reportgenerator \
        -reports:"TestResults/**/coverage.cobertura.xml" \
        -targetdir:"TestResults/coveragereport" \
        -reporttypes:"Html;Badges;TextSummary;Cobertura" \
        -verbosity:Info \
        -title:"UContentMapper Code Coverage"
    
    # Display coverage summary
    echo ""
    echo -e "${GREEN}📈 Coverage Summary:${NC}"
    echo "==================="
    if [ -f "TestResults/coveragereport/Summary.txt" ]; then
        cat TestResults/coveragereport/Summary.txt
    fi
    
    # Open coverage report if requested
    if [ "$OPEN_REPORT" = true ]; then
        if command -v xdg-open &> /dev/null; then
            xdg-open TestResults/coveragereport/index.html
        elif command -v open &> /dev/null; then
            open TestResults/coveragereport/index.html
        else
            echo -e "${YELLOW}Coverage report available at: TestResults/coveragereport/index.html${NC}"
        fi
    fi
    
    echo ""
    echo -e "${GREEN}✅ All tests passed with sufficient coverage!${NC}"
    echo -e "${BLUE}📄 Coverage Report:${NC} TestResults/coveragereport/index.html"
    echo -e "${BLUE}📄 Test Results:${NC} TestResults/test-results.trx"
else
    echo ""
    echo -e "${RED}❌ Tests failed or coverage threshold not met!${NC}"
fi

exit $TEST_EXIT_CODE