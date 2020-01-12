# Caltex Challenge

Caltex Technical Test Challenge. I left comments throughout the source code, please have a read

## Issues

There seem to be number of mistakes in the task description pdf of the coding challenge:

1. Product ids in sample requests don't match up with product ids in the product table
2. TotalAmount in sample response doesn't match up with SUM(Qty*UnitPrice) over all products in sample request
3. DiscountApplied in sample response cannot be > 0 as there is no discount promotion at TransactionDate
4. Discount promotion products table is wrong (discount for the same product is applied twice)

## Installation

1. Checkout source code
2. Open solution in Visual Studio and run the `CaltexChallenge.WebApi.Tests` test cases