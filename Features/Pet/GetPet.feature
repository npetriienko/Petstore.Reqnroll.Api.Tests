@pet
Feature: Get a pet by id
  GET /pet/{petId} returns a single pet.

  Scenario: Get an existing pet
    Given an existing pet
    When I get the pet by its id
    Then the response status code should be 200
    And the response pet should match the request

  Scenario: Get a pet that does not exist
    Given a pet that does not exist
    When I get the pet by its id
    Then the response status code should be 404
    And the error response should have code 1, type "error" and message "Pet not found"
