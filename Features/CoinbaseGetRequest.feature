Feature: CoinbaseGetRequest
This feature will test coinbase get current price response

@positiveflow
Scenario: Get current price and validate the response
	When I perform a Get request to 'bpi/currentprice.json'
	Then the response status code should be 'OK'
	Then the response body includes 'This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org'
	Then the property 'bpi' should have '3' items
	Then the property 'bpi' should contain the following child items
		| Currency |
		| USD      |
		| GBP      |
		| EUR      |
	Then the jsonpath 'bpi.GBP.description' should have value 'British Pound Sterling'

@negativeflow
Scenario: Check for test failure 1
	When I perform a Get request to 'bpi/currentprice.json'
	Then the response status code should be 'OK'
	Then the property 'bpi' should have '2' items

@negativeflow
Scenario: Check for test failure 2
	When I perform a Get request to 'bpi/currentprice.json'
	Then the response status code should be 'OK'
	Then the property 'bpi' should contain the following child items
		| Currency |
		| INR      |
		| GBP      |
		| EUR      |

@negativeflow
Scenario: Check for test failure 3
	When I perform a Get request to 'bpi/currentprice.json'
	Then the response status code should be 'OK'
	Then the jsonpath 'bpi.GBP.description' should have value 'British Pound Sterl'
