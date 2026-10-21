@pet
Feature: Add a pet
  POST /pet adds a new pet to the store.

  Scenario: Add a pet with only the required fields
    Given a valid pet with only the required fields
    When I add the pet
    Then the response status code should be 200
    And the response pet should match the request

  Scenario: Add a pet with all fields
    Given a valid pet with all fields
    When I add the pet
    Then the response status code should be 200
    And the response pet should match the request
