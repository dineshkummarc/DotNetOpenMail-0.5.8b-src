NOTES ON RUNNING THE NUNIT TESTS
--------------------------------------------------

The NUnit tests are currently set up to be compiled 
from Visual Studio.  You can get NUnit 2.2 from 
http://www.nunit.org.

In order for the tests to work, you will need to create 
a file named DotNetOpenMailTests.dll.config (you can copy the
DotNetOpenMailTests.dll.config.demo file and edit it).
This file contains a default smtp server and To and From
addresses for testing.

Note that the files DotNetOpenMailTests.dll.config and 
DotNetOpenMailTests.dll.log4net are copied  by the build 
process into the directory where the test assembly 
DotNetOpenMailTests.dll is deployed.