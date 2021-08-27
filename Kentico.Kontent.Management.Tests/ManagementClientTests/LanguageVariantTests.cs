﻿using Kentico.Kontent.Management.Models.Assets;
using Kentico.Kontent.Management.Modules.Extensions;
using Kentico.Kontent.Management.Tests.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Kentico.Kontent.Management.Models.LanguageVariants;
using Kentico.Kontent.Management.Models.Shared;
using static Kentico.Kontent.Management.Tests.ManagementClientTests.Scenario;
using Xunit.Abstractions;

namespace Kentico.Kontent.Management.Tests.ManagementClientTests
{
    [Trait("ManagementClient", "ContentItemVariant")]
    public class LanguageVariantTests
    {
        private ManagementClient _client;
        private Scenario _scenario;

        public LanguageVariantTests(ITestOutputHelper output)
        {
            //this magic can be replace once new xunit is delivered
            //https://github.com/xunit/xunit/issues/621
            var type = output.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            var test = (ITest)testMember.GetValue(output);

            _scenario = new Scenario(test.TestCase.TestMethod.Method.Name);
            _client = _scenario.Client;
        }

        [Fact]
        public async Task UpsertVariant_ById_LanguageId_UpdatesVariant()
        {
            var contentItemVariantUpsertModel = new ContentItemVariantUpsertModel() { Elements = _scenario.Elements };

            var itemIdentifier = Reference.ById(EXISTING_ITEM_ID);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariantUpsertModel);

            Assert.Equal(EXISTING_ITEM_ID, responseVariant.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);
        }

        [Fact]
        public async Task UpsertVariant_ByCodename_LanguageId_UpdatesVariant()
        {
            var contentItemVariantUpsertModel = new ContentItemVariantUpsertModel() { Elements = _scenario.Elements };

            var itemIdentifier = Reference.ByCodename(EXISTING_ITEM_CODENAME);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariantUpsertModel);

            Assert.Equal(EXISTING_ITEM_ID, responseVariant.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);
        }

        [Fact]
        public async Task UpsertVariant_ById_LanguageCodename_UpdatesVariant()
        {
            var contentItemVariantUpsertModel = new ContentItemVariantUpsertModel() { Elements = _scenario.Elements };

            var itemIdentifier = Reference.ById(EXISTING_ITEM_ID);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariantUpsertModel);

            Assert.Equal(EXISTING_ITEM_ID, responseVariant.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);
        }

        [Fact]
        public async Task UpsertVariant_ByCodename_LanguageCodename_UpdatesVariant()
        {
            var contentItemVariantUpsertModel = new ContentItemVariantUpsertModel() { Elements = _scenario.Elements };

            var itemIdentifier = Reference.ByCodename(EXISTING_ITEM_CODENAME);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariantUpsertModel);

            Assert.Equal(EXISTING_ITEM_ID, responseVariant.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);
        }

        [Fact]
        public async Task UpsertVariant_ByExternalId_LanguageCodename_UpdatesVariant()
        {
            // Arrange
            var externalId = "fe2e8c24f0794f01b36807919602625d";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, preparedItem);

            // Test
            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);

            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);
            var contentItemVariantUpsertModel = new ContentItemVariantUpsertModel() { Elements = _scenario.Elements };
            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariantUpsertModel);

            Assert.Equal(responseVariant.Language.Id, EXISTING_LANGUAGE_ID);
            AssertResponseElements(responseVariant);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task UpsertVariant_ByExternalId_LanguageCodename_CreatesVariant()
        {
            // Arrange
            var externalId = "348052a5ad8c44ddac1e9683923d74a5";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);

            // Test
            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);

            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);
            var contentItemVariantUpsertModel = new ContentItemVariantUpsertModel() { Elements = _scenario.Elements };
            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariantUpsertModel);

            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task UpsertVariant_ByExternalId_LanguageId_UpdatesVariant()
        {
            // Arrange
            var externalId = "d5e050980baa43b085b909cdea4c6d2b";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, preparedItem);

            // Test
            var contentItemVariantUpsertModel = new ContentItemVariantUpsertModel() { Elements = _scenario.Elements };

            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariantUpsertModel);

            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task UpsertVariant_ByExternalId_LanguageId_CreatesVariant()
        {
            // Arrange
            var externalId = "73e02811b05f429284006ea94c68c8f7";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);

            // Test
            var contentItemVariantUpsertModel = new ContentItemVariantUpsertModel() { Elements = _scenario.Elements };

            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariantUpsertModel);

            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task UpsertVariant_UsingResponseModel_UpdatesVariant()
        {
            // Arrange
            var externalId = "4357b71d21eb45369d54a635faf7672b";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            var emptyElements = new List<object>();
            var preparedVariant = await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, emptyElements, preparedItem);

            // Test
            preparedVariant.Elements = _scenario.Elements;
            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariant: preparedVariant);

            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task UpsertVariant_UsingResponseModel_CreatesVariant()
        {
            // Arrange
            var externalId = "5249f596a8be4d719bc9816e3d416d16";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            var emptyElements = new List<object>();
            var preparedVariant = await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, emptyElements, preparedItem);

            // Test
            preparedVariant.Elements = _scenario.Elements;
            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ById(Guid.Empty);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, contentItemVariant: preparedVariant);

            Assert.Equal(Guid.Empty, responseVariant.Language.Id);
            AssertResponseElements(responseVariant);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task ListContentItemVariants_ById_ListsVariants()
        {
            var identifier = Reference.ById(EXISTING_ITEM_ID);

            var responseVariants = await _client.ListContentItemVariantsAsync(identifier);

            Assert.Equal(EXISTING_ITEM_ID, responseVariants.First().Item.Id);
        }

        [Fact]
        public async Task ListContentItemVariants_ByCodename_ListsVariants()
        {
            var identifier = Reference.ByCodename(EXISTING_ITEM_CODENAME);

            var responseVariants = await _client.ListContentItemVariantsAsync(identifier);

            Assert.Equal(EXISTING_ITEM_ID, responseVariants.First().Item.Id);
        }

        [Fact]
        public async Task ListContentItemVariants_ByExternalId_ListsVariants()
        {
            // Arrange
            var externalId = "0220e6ec5b77401ea113b5273c8cdd5e";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, preparedItem);

            // Test
            var identifier = Reference.ByExternalId(externalId);
            var responseVariants = await _client.ListContentItemVariantsAsync(identifier);

            Assert.Single(responseVariants);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task GetContentItemVariant_ById_LanguageId_GetsVariant()
        {
            var itemIdentifier = Reference.ById(EXISTING_ITEM_ID);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var response = await _client.GetContentItemVariantAsync(identifier);

            Assert.NotNull(response);
            Assert.Equal(EXISTING_ITEM_ID, response.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, response.Language.Id);
        }

        [Fact]
        public async Task GetContentItemVariant_ById_LanguageCodename_GetsVariant()
        {
            var itemIdentifier = Reference.ById(EXISTING_ITEM_ID);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var response = await _client.GetContentItemVariantAsync(identifier);

            Assert.NotNull(response);
            Assert.Equal(EXISTING_ITEM_ID, response.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, response.Language.Id);
        }

        [Fact]
        public async Task GetContentItemVariant_ByCodename_LanguageId_GetsVariant()
        {
            var itemIdentifier = Reference.ByCodename(EXISTING_ITEM_CODENAME);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var response = await _client.GetContentItemVariantAsync(identifier);

            Assert.NotNull(response);
            Assert.Equal(EXISTING_ITEM_ID, response.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, response.Language.Id);
        }

        [Fact]
        public async Task GetContentItemVariant_ByCodename_LanguageCodename_GetsVariant()
        {
            var itemIdentifier = Reference.ByCodename(EXISTING_ITEM_CODENAME);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var response = await _client.GetContentItemVariantAsync(identifier);

            Assert.NotNull(response);
            Assert.Equal(EXISTING_ITEM_ID, response.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, response.Language.Id);
        }

        [Fact]
        public async Task GetContentItemVariant_ByExternalId_LanguageCodename_GetsVariant()
        {
            // Arrange
            var externalId = "f9cfaa3e00f64e22a144fdacf4cba3e5";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, preparedItem);

            // Test
            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var response = await _client.GetContentItemVariantAsync(identifier);

            Assert.NotNull(response);
            Assert.Equal(preparedItem.Id, response.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, response.Language.Id);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task GetContentItemVariant_ByExternalId_ReturnsVariant()
        {
            var externalId = "ad66f70ed9bb4b8694116c9119c4a930";
            var preparedItem = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, preparedItem);

            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var response = await _client.GetContentItemVariantAsync(identifier);

            Assert.NotNull(response);
            Assert.Equal(preparedItem.Id, response.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, response.Language.Id);

            // Cleanup
            var itemToClean = Reference.ByExternalId(externalId);
            await _client.DeleteContentItemAsync(itemToClean);
        }

        [Fact]
        public async Task DeleteContentItemVariant_ById_LanguageCodename_DeletesVariant()
        {
            var itemResponse = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, itemResponse);

            var itemIdentifier = Reference.ById(itemResponse.Id);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            await _client.DeleteContentItemVariantAsync(identifier);
        }

        [Fact]
        public async Task DeleteContentItemVariant_ById_LanguageId_DeletesVariant()
        {
            var itemResponse = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, itemResponse);

            var itemIdentifier = Reference.ById(itemResponse.Id);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            await _client.DeleteContentItemVariantAsync(identifier);
        }

        [Fact]
        public async Task DeleteContentItemVariant_ByCodename_LanguageId_DeletesVariant()
        {
            // Prepare item
            var itemResponse = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, itemResponse);

            var itemIdentifier = Reference.ByCodename(itemResponse.Codename);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            await _client.DeleteContentItemVariantAsync(identifier);
        }

        [Fact]
        public async Task DeleteContentItemVariant_ByCodename_LanguageCodename_DeletesVariant()
        {
            // Prepare item
            var itemResponse = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, itemResponse);

            var itemIdentifier = Reference.ByCodename(itemResponse.Codename);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            await _client.DeleteContentItemVariantAsync(identifier);
        }

        [Fact]
        public async Task DeleteContentItemVariant_ByExternalId_LanguageId_DeletesVariant()
        {
            var externalId = "90285b1a983c43299638c8a835f16b81";
            var itemResponse = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, itemResponse);

            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            await _client.DeleteContentItemVariantAsync(identifier);
        }

        [Fact]
        public async Task DeleteContentItemVariant_ByExternalId_LanguageCodename_DeletesVariant()
        {
            var externalId = "f4fe87222b6b46739bc673f6e5165c12";
            var itemResponse = await TestUtils.PrepareTestItem(_client, EXISTING_CONTENT_TYPE_CODENAME, externalId);
            await TestUtils.PrepareTestVariant(_client, EXISTING_LANGUAGE_CODENAME, _scenario.Elements, itemResponse);

            var itemIdentifier = Reference.ByExternalId(externalId);
            var languageIdentifier = Reference.ByCodename(EXISTING_LANGUAGE_CODENAME);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            await _client.DeleteContentItemVariantAsync(identifier);
        }

        [Fact]
        public async Task ListStronglyTypedContentItemVariants_ById_ListsVariants()
        {
            var identifier = Reference.ById(EXISTING_ITEM_ID);

            var responseVariants = await _client.ListContentItemVariantsAsync<ComplexTestModel>(identifier);

            Assert.All(responseVariants, x =>
            {
                Assert.NotNull(x.Elements);
            });
            Assert.Equal(EXISTING_ITEM_ID, responseVariants.First().Item.Id);
        }

        [Fact]
        public async Task GetStronglyTypedContentItemVariantAsync_ById_LanguageId_GetVariant()
        {
            var itemIdentifier = Reference.ById(EXISTING_ITEM_ID);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var response = await _client.GetContentItemVariantAsync<ComplexTestModel>(identifier);

            Assert.NotNull(response);
            Assert.Equal(EXISTING_ITEM_ID, response.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, response.Language.Id);
            Assert.NotNull(response.Elements);
        }

        [Fact]
        public async Task UpsertStronglyTypedContentItemVariantAsync_ById_LanguageId_UpdatesVariant()
        {
            var itemIdentifier = Reference.ById(EXISTING_ITEM_ID);
            var languageIdentifier = Reference.ById(EXISTING_LANGUAGE_ID);
            var identifier = new ContentItemVariantIdentifier(itemIdentifier, languageIdentifier);

            var responseVariant = await _client.UpsertContentItemVariantAsync(identifier, _scenario.StronglyTypedElements);

            Assert.Equal(EXISTING_ITEM_ID, responseVariant.Item.Id);
            Assert.Equal(EXISTING_LANGUAGE_ID, responseVariant.Language.Id);
            Assert.NotNull(responseVariant.Elements);
            AssertStronglyTypedResponseElements(responseVariant.Elements);
        }

        private (dynamic expected, dynamic actual) GetElementByCodename(string codename, IEnumerable<dynamic> actualElements)
        {
            var expected = _scenario.Elements.Single(x => x.codename == codename);
            var actual = actualElements.Single(x => x.element.id == expected.element.id);

            return (expected, actual);
        }

        private string UnifyWhitespace(string text)
        {
            return Regex.Replace(text, "\\s+", string.Empty);
        }

        private void AssertResponseElements(ContentItemVariantModel responseVariant)
        {
            var (expected, actual) = GetElementByCodename("title", responseVariant.Elements);
            Assert.Equal(expected.value, actual.value);

            (expected, actual) = GetElementByCodename("post_date", responseVariant.Elements);
            Assert.Equal(expected.value, actual.value);

            (expected, actual) = GetElementByCodename("url_pattern", responseVariant.Elements);
            Assert.Equal(expected.mode, actual.mode);
            Assert.Equal(expected.value, actual.value);

            (expected, actual) = GetElementByCodename("body_copy", responseVariant.Elements);
            Assert.Equal(UnifyWhitespace(expected.value), UnifyWhitespace(actual.value));

            // TODO check component of the rich text element

            (expected, actual) = GetElementByCodename("related_articles", responseVariant.Elements);
            Assert.Equal(EXISTING_ITEM_ID, Guid.Parse((actual.value as IEnumerable<dynamic>)?.First().id));

            (expected, actual) = GetElementByCodename("personas", responseVariant.Elements);
            Assert.Equal(EXISTING_TAXONOMY_TERM_ID, Guid.Parse((actual.value as IEnumerable<dynamic>)?.First().id));

            (expected, actual) = GetElementByCodename("teaser_image", responseVariant.Elements);
            Assert.Equal(EXISTING_ASSET_ID, Guid.Parse((actual.value as IEnumerable<dynamic>)?.First().id));

            (expected, actual) = GetElementByCodename("options", responseVariant.Elements);
            Assert.Equal(EXISTING_MULTIPLE_CHOICE_OPTION_ID_PAID, Guid.Parse((actual.value as IEnumerable<dynamic>)?.First().id));
            Assert.Equal(EXISTING_MULTIPLE_CHOICE_OPTION_ID_FEATURED, Guid.Parse((actual.value as IEnumerable<dynamic>)?.Skip(1).First().id));
        }

        private void AssertStronglyTypedResponseElements(ComplexTestModel elements)
        {
            Assert.Equal(_scenario.StronglyTypedElements.Title.Value, elements.Title.Value);
            Assert.Equal(_scenario.StronglyTypedElements.PostDate.Value, elements.PostDate.Value);
            // TODO extend for complex elements
            // Assert.Equal(UnifyWhitespace(StronglyTypedElements.BodyCopy), UnifyWhitespace(elements.BodyCopy));
            Assert.Equal(_scenario.StronglyTypedElements.UrlPattern.Mode, elements.UrlPattern.Mode);
            Assert.Equal(_scenario.StronglyTypedElements.UrlPattern.Value, elements.UrlPattern.Value);
            Assert.NotNull(elements.TeaserImage.Value);
            Assert.Equal(_scenario.StronglyTypedElements.TeaserImage.Value.FirstOrDefault()?.Id, elements.TeaserImage.Value.FirstOrDefault()?.Id);
            Assert.NotNull(elements.Options.Value);
            Assert.NotEmpty(elements.Options.Value);
            Assert.Equal(_scenario.StronglyTypedElements.Options.Value.Select(option => option.Id), elements.Options.Value.Select(option => option.Id));
            Assert.Contains(EXISTING_MULTIPLE_CHOICE_OPTION_ID_PAID, elements.Options.Value.Select(option => option.Id));
            Assert.Contains(EXISTING_MULTIPLE_CHOICE_OPTION_ID_FEATURED, elements.Options.Value.Select(option => option.Id));

            Assert.Single(elements.RelatedArticles.Value);
            Assert.Equal(EXISTING_ITEM_ID, elements.RelatedArticles.Value.First().Id);

            Assert.Single(elements.Personas.Value);
            Assert.Equal(EXISTING_TAXONOMY_TERM_ID, elements.Personas.Value.FirstOrDefault()?.Id);
        }
    }
}
