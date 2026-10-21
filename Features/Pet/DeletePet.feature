@pet
Feature: Delete a pet
  DELETE /pet/{petId} removes a pet from the store.

  Scenario: Delete an existing pet
    Given an existing pet
    When I delete the pet
    Then the response status code should be 200
    And the response message should be the pet id
    And the pet should not be retrievable by its id

  Scenario: Delete a pet that does not exist
    Given a pet id that does not exist
    When I delete the pet
    Then the response status code should be 404
    And the response body should be empty
