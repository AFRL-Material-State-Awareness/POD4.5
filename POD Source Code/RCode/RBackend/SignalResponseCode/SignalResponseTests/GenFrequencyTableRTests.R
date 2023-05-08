#The purpose of this script is to test the functions that were called wthin 

library(testthat)
library(magrittr)
library(stringr)
library(rstudioapi)
# Define the setup function
setup <- function(){
  # Code that sets up the environment for the tests
  current_file <<- dirname(rstudioapi::getSourceEditorContext()$path)
  myClass=paste(current_file, "/../GenFrequencyTableR.R",sep = "")
  source(myClass)
  normalityTableGen <<- GenerateNormalityTable$new()
}

context("GenerateNormalityTable.R Tests")
# Tests for RoundUpNice = function(x, nice=c(1,2,4,5,6,8,10))
# Only positve values should be passed here
# Will round a decimal or integer to a nice round number
# if the number is greater than 10, then rounds to 20 (floor(log10(10.001))= 1)
# if the number is greater than 200, then rounds to 200 (floor(log10(100.5))= 2)
# etc
test_that("RoundUpNice_SinglePositiveNumberPassedGreaterOrEqualToThan1_ReturnsAnIntegerRoundsUpToLargestDigit", {
  setup()
  #Arrange
  input= c(1.0, 1.123, 10.001, 200.5, 450, 559.6,701.1, 905.314)
  expected_value = c(1, 2, 20, 400, 500, 600, 800, 1000)
  for(i in 1:length(input)){
    #Act
    result=normalityTableGen$RoundUpNice(input[i])
    # Assert that the result has no decimals
    expect_equal(result, floor(result))
    # Test a method of MyClass
    expect_equal(result, expected_value[i])
  }
})
# The NearestNonZeroDigit refers to the fact that the function will round to the smallest non-zero digit
# in other words .15 round to .1 and .015 round to .01
# numbers that are 
test_that("RoundUpNice_NumberBetween0And1Passed_ReturnsValueRoundedUpToTheNearestNonZeroDigit", {
  setup()
  #Arrange
  input= c(.0321, .0009, .123,.456,.789, .8, .801)
  expected_value = c(.04, 0.001,.2, .5, .8, .8, 1.0)
  for(i in 1:length(input)){
    #Act
    result=normalityTableGen$RoundUpNice(input[i])
    # Test a method of MyClass
    expect_equal(result, expected_value[i])
  }
})
test_that("RoundUpNice_VectorPassedInsteadOfSingleNumber_ThrowsAnError", {
  setup()
  #Arrange
  input= c(1,2,3)
  #Act
  expect_error(normalityTableGen$RoundUpNice(input))

})
test_that("RoundUpNice_ZeroOrNegativeNumberPassed_ThrowsAnError", {
  setup()
  #Arrange
  input= c(0,-1)
  #Act
  expect_error(normalityTableGen$RoundUpNice(input))
  
})
# Tests for RoundUpNiceNeg = function(x, nice=c(10,8,6,5,4,2,1))
# similar as previous function except for negative numbers
# returns a rounded decimal when between -1 and 0 (will return an error if the step size is rounded to 0)
test_that("RoundUpNiceNeg_SingleNumberLessThanOrEqualTo1Passed_ReturnsANegativeIntegerRoundsUpToLargestNegDigit", {
  setup()
  #Arrange
  input=  c(-1.0, -1.123, -10.001, -200.5, -450, -559.6,-799.9, -999, -1000.001)
  expected_value =  c(-1, -1, -10, -200, -400, -500, -600, -800, -1000)
  for(i in 1:length(input)){
    #Act
    result=normalityTableGen$RoundUpNiceNeg(input[i])
    # Assert that the result has no decimals
    expect_equal(result, floor(result))
    # Test a method of MyClass
    expect_equal(result, expected_value[i])
  }
})
test_that("RoundUpNiceNeg_NumberBetweenNegative1And0Passed_ReturnsValueRoundedUpToTheNearestNonZeroDigit", {
  setup()
  #Arrange
  input= c(-.0321, -.0009, -.123,-.456,-.789, -.8, -.999)
  expected_value = c(-.02, -.0008,-.1,-.4, -.6, -.8, -.8)
  for(i in 1:length(input)){
    #Act
    result=normalityTableGen$RoundUpNiceNeg(input[i])
    # Test a method of MyClass
    expect_equal(result, expected_value[i])
  }
})
test_that("RoundUpNiceNeg_VectorPassedInsteadOfSingleNumber_ThrowsAnError", {
  setup()
  #Arrange
  input= c(-1,-2,-3)
  #Act
  expect_error(normalityTableGen$RoundUpNiceNeg(input))
  
})
test_that("RoundUpNiceNeg_ZeroOrPositiveNumberPassedIntoFunction_ThrowsAnError", {
  setup()
  #Arrange
  input= c(0,1)
  #Act
  for(i in 1:length(input)){
    expect_error(normalityTableGen$RoundUpNiceNeg(input[i]))
  }
})

##### Round down functions

# Tests for RoundDownNice = function(x, nice=c(10,8,6,5,4,2,1))
# Only positve values should be passed here
# Will round a decimal or integer to a nice round number except will get rounded down
# if the number is 19, then rounds to 10 
# if the number is 999, then rounds to 800 
# etc
test_that("RoundDownNice_SinglePositiveNumberPassedGreaterOrEqualToThan1_ReturnsAnIntegerRoundsDownToLargestDigit", {
  setup()
  #Arrange
  input= c(1.0, 1.123, 19.999, 200.5, 450, 599.9,701.1, 905.314, 1998)
  expected_value = c(1, 1, 10, 200, 400, 500, 600, 800, 1000)
  for(i in 1:length(input)){
    #Act
    result=normalityTableGen$RoundDownNice(input[i])
    # Assert that the result has no decimals
    expect_equal(result, floor(result))
    # Assert the expected value a method of MyClass
    expect_equal(result, expected_value[i])
  }
})
test_that("RoundDownNice_NumberBetween0And1Passed_ReturnsValueRoundedDownToTheNearestNonZeroDigit", {
  setup()
  #Arrange
  input= c(.00101, .009, .123,.456,.789, .8, .999)
  expected_value = c(.001,.008, .1,.4, .6, .8, .8)
  for(i in 1:length(input)){
    #Act
    result=normalityTableGen$RoundDownNice(input[i])
    # Test a method of MyClass
    expect_equal(result, expected_value[i])
  }
})
test_that("RoundDownNice_VectorPassedInteadOfNumber_ThrowsError", {
  setup()
  #Arrange
  input= c(1,2,3)
  expect_error(normalityTableGen$RoundUpNice(input))
})
test_that("RoundDownNice_NumberIs0OrNegative_ThrowsError", {
  setup()
  #Arrange
  input= c(0, -1)
  for(i in 1:length(input)){
    expect_error(normalityTableGen$RoundUpNice(input[i]))
  }
})
# Tests for RoundDownNiceNeg  = function(x, nice=c(1,2,4,5,6,8,10))
# similar as RoundDownNice function except for negative numbers
# returns a rounded decimal when between -1 and 0 (will return an error if the step size is rounded to 0)
test_that("RoundDownNiceNeg_NumberLessThanOrEqualToNegative1Passed_ReturnsANegativeIntegerRoundsDownToSmallestNegDigit", {
  setup()
  #Arrange
  input=  c(-1.0, -1.123, -10.001, -200.5, -450, -559.6,-600.00, -999, -1000.001)
  expected_value =  c(-1, -2, -20, -400, -500, -600, -600, -1000, -2000)
  for(i in 1:length(input)){
    #Act
    result=normalityTableGen$RoundDownNiceNeg(input[i])
    # Assert that the result has no decimals
    expect_equal(result, floor(result))
    # Test a method of MyClass
    expect_equal(result, expected_value[i])
  }
})

test_that("RoundDownNiceNeg_NumberBetweenNegative1And0Passed_ReturnsNumberRoundedDOWNToTheNearestTenth", {
  setup()
  #Arrange
  input= c(-.00101, -.009, -.123,-.456,-.789, -.8, -.999)
  expected_value = c(-.002, -.01,-.2,-.5, -.8, -.8, -1.0)
  for(i in 1:length(input)){
    #Act
    result=normalityTableGen$RoundDownNiceNeg(input[i])
    # Test a method of MyClass
    expect_equal(result, expected_value[i])
  }
})

test_that("RoundUpNiceNeg_VectorPassedInsteadOfSingleNumber_ThrowsAnError", {
  setup()
  #Arrange
  input= c(-1,-2,-3)
  #Act
  expect_error(normalityTableGen$RoundDownNiceNeg(input))
  
})
test_that("RoundUpNiceNeg_ZeroOrPositiveNumberPassedIntoFunction_ThrowsAnError", {
  setup()
  #Arrange
  input= c(0,1)
  #Act
  for(i in 1:length(input)){
    expect_error(normalityTableGen$RoundDownNiceNeg(input[i]))
  }
})

