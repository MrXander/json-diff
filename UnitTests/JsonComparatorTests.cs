using System.Collections.Generic;
using System.Linq;
using JsonDiff;
using Xunit;

namespace UnitTests;

public class JsonComparatorTests
{
    private readonly JsonComparator _comparator;

    public JsonComparatorTests()
    {
        _comparator = new JsonComparator();
    }

    //[MethodUnderTest]_[Scenario]_[ExpectedResult]

    [Fact]
    public void Diff_AddOneProperty_AddedHasValue()
    {
        //Arrange


        //Act
        var diff = _comparator.GetDiff("{ \"name\": \"John\" }", "{ \"name\": \"John\", \"surname\": \"Surname_John\" }", null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Removed);
        Assert.Empty(diff.Changed);

        Assert.NotEmpty(diff.Added);

        Assert.Equal(1, diff.Added.Count);
        Assert.Equal("/surname", diff.Added.First().Path);
    }

    [Fact]
    public void Diff_AddOnePropertyAndChangeAnotherPropertyValue_AddedHasChangedAndChangedHasValue()
    {
        //Arrange


        //Act
        var diff = _comparator.GetDiff("{ \"name\": \"John\" }", "{ \"name\": \"NEW_John\", \"surname\": \"Surname_John\" }", null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Removed);

        Assert.NotEmpty(diff.Changed);
        Assert.NotEmpty(diff.Added);

        Assert.Equal(1, diff.Added.Count);
        Assert.Equal("/surname", diff.Added.First().Path);

        Assert.Equal(1, diff.Changed.Count);
        Assert.Equal("/name", diff.Changed.First().Path);
        Assert.Equal("NEW_John", diff.Changed.First().NewValue);
    }

    [Fact]
    public void Diff_ChangePropValue_ChangedHasValue()
    {
        //Arrange


        //Act
        var diff = _comparator.GetDiff("{ \"name\": \"Justin\" }", "{ \"name\": \"John\" }", null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotEmpty(diff.Changed);
        Assert.Equal(1, diff.Changed.Count);
        Assert.Equal("/name", diff.Changed.First().Path);
        Assert.Equal("Justin", diff.Changed.First().Value);
        Assert.Equal("John", diff.Changed.First().NewValue);

        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Added);
        Assert.Empty(diff.Removed);
    }

    [Fact]
    public void Diff_CompareEqualArrayOfValuesHavingDifferentOrderOfElements_NoDiff()
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff("{ \"arr\": [5,3,1,4,2] }", "{ \"arr\": [1,2,3,4,5] }", null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Removed);
        Assert.Empty(diff.Changed);
        Assert.Empty(diff.Added);
    }

    [Fact]
    public void Diff_CompareArraysOfValuesNewElementAdded_AddedHasValue()
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff("{ \"arr\": [5,3,1,4,2] }", "{ \"arr\": [1,2,3,4,5,6] }", null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Removed);
        Assert.Empty(diff.Changed);

        Assert.NotEmpty(diff.Added);
        Assert.Equal("/arr/5", diff.Added.First().Path);
        Assert.Equal("6", diff.Added.First().Value);
    }

    [Fact]
    public void Diff_CompareArraysOfValuesElementDeleted_RemovedHasValue()
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff("{ \"arr\": [5,3,1,4,2] }", "{ \"arr\": [1,2,3,5] }", null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Added);
        Assert.Empty(diff.Changed);

        Assert.NotEmpty(diff.Removed);
        Assert.Equal("/arr/3", diff.Removed.First().Path);
        Assert.Equal("4", diff.Removed.First().Value);
    }

    [Fact]
    public void Diff_CompareArraysOfValuesValueReplaced_AddedAndRemovedHaveValue()
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff("{ \"arr\": [5,3,1,4,2] }", "{ \"arr\": [1,2,3,4,6] }", null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Changed);

        Assert.NotEmpty(diff.Added);
        Assert.NotEmpty(diff.Removed);

        Assert.Equal(1, diff.Added.Count);
        Assert.Equal(1, diff.Removed.Count);

        Assert.Equal("/arr/4", diff.Removed.First().Path);
        Assert.Equal("5", diff.Removed.First().Value);

        Assert.Equal("/arr/4", diff.Added.First().Path);
        Assert.Equal("6", diff.Added.First().Value);
    }

    [Theory]
    [InlineData(@"{ ""test_obj"": {  ""arr"": [ 5, 3, 1, 4, 2 ]  } }",
        @"{ ""test_obj"": {  ""arr"": [ 1, 2, 3, 5, 4 ]  } }")]
    [InlineData(@"{ ""test_obj"": {  ""inner_obj"": { ""arr"": [ 5, 3, 1, 4, 2 ]  } } }",
        @"{ ""test_obj"": {  ""inner_obj"": { ""arr"": [ 1, 2, 3, 5, 4 ]  } } }")]
    [InlineData(@"{ ""test_obj"": {  ""inner_obj"": { ""inner_inner_obj"": { ""arr"": [ 5, 3, 1, 4, 2 ] }  } } }",
        @"{ ""test_obj"": {  ""inner_obj"": { ""inner_inner_obj"": { ""arr"": [ 1, 2, 3, 5, 4 ] }  } } }")]
    public void Diff_CompareEqualLevelNestedArraysOfValues_NoDiff(string srcJson, string destJson)
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff(srcJson, destJson, null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Changed);
        Assert.Empty(diff.Added);
        Assert.Empty(diff.Removed);
    }

    [Theory]
    [InlineData(@"{ ""test_obj"": {  ""inner_obj"": { ""inner_inner_obj"": { ""arr"": [ 5, 3, 1, 4, 2 ] }  } } }",
        @"{ ""test_obj"": {  ""inner_obj"": { ""inner_inner_obj"": { ""arr"": [ 1, 2, 3, 5, 4, 6 ] }  } } }")]
    public void Diff_CompareEqualLevel3NestedArraysOfValues_AddedHasValue(string srcJson, string destJson)
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff(srcJson, destJson, null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Changed);

        Assert.NotEmpty(diff.Added);

        Assert.Equal(1, diff.Added.Count);

        Assert.Equal("/test_obj/inner_obj/inner_inner_obj/arr/5", diff.Added.First().Path);
        Assert.Equal("6", diff.Added.First().Value);
    }

    [Theory]
    [InlineData(@"{ ""test_obj"": {  ""inner_obj"": { ""inner_inner_obj"": { ""arr"": [ 5, 3, 1, 4, 2 ] }  } } }",
        @"{ ""test_obj"": {  ""inner_obj"": { ""inner_inner_obj"": { ""arr"": [ 1, 2, 3, 4, 6 ] }  } } }")]
    public void Diff_CompareEqualLevel3NestedArraysOfValues_AddedAndRemovedHaveValue(string srcJson, string destJson)
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff(srcJson, destJson, null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Changed);

        Assert.NotEmpty(diff.Added);
        Assert.NotEmpty(diff.Removed);

        Assert.Equal(1, diff.Added.Count);
        Assert.Equal(1, diff.Removed.Count);

        Assert.Equal("/test_obj/inner_obj/inner_inner_obj/arr/4", diff.Removed.First().Path);
        Assert.Equal("5", diff.Removed.First().Value);

        Assert.Equal("/test_obj/inner_obj/inner_inner_obj/arr/4", diff.Added.First().Path);
        Assert.Equal("6", diff.Added.First().Value);
    }

    [Theory]
    [InlineData(@"{ ""test_obj"": {  ""arr"": [ { ""id"": 3 }, { ""id"": 2 }, { ""id"": 5 }, { ""id"": 4 }, { ""id"": 1 }  ] } }",
        @"{ ""test_obj"": {  ""arr"": [ { ""id"": 1 }, { ""id"": 2 }, { ""id"": 5 }, { ""id"": 4 }, { ""id"": 3 }  ] } }")]
    [InlineData(@"{ ""test_obj"": {  ""nested_obj"": { ""arr"": [ { ""id"": 3 }, { ""id"": 2 }, { ""id"": 5 }, { ""id"": 4 }, { ""id"": 1 } ]  } } }",
        @"{ ""test_obj"": {  ""nested_obj"": { ""arr"": [ { ""id"": 1 }, { ""id"": 2 }, { ""id"": 5 }, { ""id"": 4 }, { ""id"": 3 } ] } } }")]
    [InlineData(
        @"{ ""test_obj"": {  ""nested_obj"": { ""nested_nested_obj"": { ""arr"": [ {  ""id"": 3 }, {  ""id"": 2 }, {  ""id"": 5 }, {  ""id"": 4 }, {  ""id"": 1 } ] }  } } }",
        @"{ ""test_obj"": {  ""nested_obj"": { ""nested_nested_obj"": { ""arr"": [ {  ""id"": 1 }, {  ""id"": 2 }, {  ""id"": 5 }, {  ""id"": 4 }, {  ""id"": 3 } ] }  } } }")]
    public void Diff_CompareEqualLevelNestedArraysOfObjects_NoDiff(string srcJson, string destJson)
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff(srcJson, destJson, null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Changed);
        Assert.Empty(diff.Added);
        Assert.Empty(diff.Removed);
    }

    [Theory]
    [InlineData(@"{ ""test_obj"": ""test_value"", ""null_prop"": null }",
        @"{ ""test_obj"": ""test_value"" }")]
    public void Diff_NullPropsDissapearedInNewJson_NoDiff(string srcJson, string destJson)
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff(srcJson, destJson, null);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Changed);
        Assert.Empty(diff.Added);
        Assert.Empty(diff.Removed);
    }
    
    [Theory]
    [InlineData(@"{ ""test_obj"": ""test_value"", ""null_prop"": null }",
        @"{ ""test_obj"": ""test_value"" }")]
    public void Diff_NullPropsDissapearedInNewJson_HasDiff(string srcJson, string destJson)
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff(srcJson, destJson, null, false);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Changed);
        Assert.Empty(diff.Added);
        
        Assert.NotEmpty(diff.Removed);
        Assert.Equal(1, diff.Removed.Count);
        Assert.Equal("/null_prop", diff.Removed.First().Path);
        Assert.Equal(string.Empty, diff.Removed.First().Value);
    }
    
    [Theory]
    [InlineData(@"{ ""test_obj"": ""test_value"", ""prop_to_ignore"": ""ignore_me"" }",
        @"{ ""test_obj"": ""test_value"", ""prop_to_ignore"": ""changed_ignore_me"" }",
        new [] {"/prop_to_ignore"})]
    [InlineData(@"{ ""test_obj"": ""test_value"", ""prop_to_ignore"": {  ""nested_ignore_1"": 12,  ""nested_ignore_2"": 34,  ""nested_ignore_arr"": [ 1, 2, 3, 4  ] } }",
        @"{ ""test_obj"": ""test_value"", ""prop_to_ignore"": {  ""nested_ignore_1"": 56,  ""nested_ignore_2"": 34,  ""nested_ignore_arr"": [ 1, 2, 3, 4  ] } }",
        new [] {"/prop_to_ignore/nested_ignore_1"})]
    [InlineData(@"{ ""test_obj"": ""test_value"", ""prop_to_ignore"": {  ""nested_ignore_1"": 12,  ""nested_ignore_2"": 34,  ""nested_ignore_arr"": [ 1, 2, 3, 4  ] } }",
        @"{ ""test_obj"": ""test_value"", ""prop_to_ignore"": {  ""nested_ignore_1"": 56,  ""nested_ignore_2"": 78,  ""nested_ignore_arr"": [ 5, 2, 3, 4  ] } }",
        new [] {"/prop_to_ignore/*"})]
    [InlineData(@"{ ""test_obj"": ""test_value"", ""prop_to_ignore"": {  ""nested_ignore_1"": 12,  ""nested_ignore_2"": 34,  ""nested_ignore_arr"": [ 1, 2, 3, 4  ] } }",
        @"{ ""test_obj"": ""test_value"", ""prop_to_ignore"": {  ""nested_ignore_1"": 12,  ""nested_ignore_2"": 34,  ""nested_ignore_arr"": [ 0, 2, 3, 4  ] } }",
        new [] {"/prop_to_ignore/nested_ignore_arr/0"})]
    public void Diff_IgnorePathsByRegex_NoDiff(string srcJson, string destJson, IList<string> ignorePaths)
    {
        //Arrange

        //Act
        var diff = _comparator.GetDiff(srcJson, destJson, ignorePaths);

        //Assert
        Assert.NotNull(diff);
        Assert.NotNull(diff.Changed);
        Assert.NotNull(diff.Added);
        Assert.NotNull(diff.Removed);
        Assert.Empty(diff.Changed);
        Assert.Empty(diff.Added);
        Assert.Empty(diff.Removed);
    }
}