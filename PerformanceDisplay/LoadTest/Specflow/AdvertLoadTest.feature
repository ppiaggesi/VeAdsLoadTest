Feature: AdvertLoadTest
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: WebTime performance per advert
	Given I have navigated to advert link
	When I get the webTimings for our performance measurements
	Then the webTiming result should be greater than zero
