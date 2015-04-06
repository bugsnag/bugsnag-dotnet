MDTOOL ?= /Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool

.PHONY: all clean

all: Bugsnag.dll

Bugsnag.dll:
	$(MDTOOL) build -c:Release Bugsnag-XamarinStudio.sln

clean:
	$(MDTOOL) build -t:Clean -c:Release Bugsnag-XamarinStudio.sln
