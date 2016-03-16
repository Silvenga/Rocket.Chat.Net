namespace Rocket.Chat.Net.Tests.Models
{
    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Driver;

    using Xunit;

    public class StreamCollectionFacts
    {
        private readonly Fixture _autoFixture = new Fixture();

        [Fact]
        public void When_object_is_added_and_collection_is_empty_add_new_object()
        {
            var item = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };

            var collection = new StreamCollection();

            // Act
            collection.Added(item.Id, item);

            // Assert
            var result = collection.GetAnonymousTypeById(item.Id, item);

            result.Should().Be(item);
        }

        [Fact]
        public void When_object_exists_object_should_exist()
        {
            var item = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };

            var collection = new StreamCollection();

            // Act
            collection.Added(item.Id, item);
            var exists = collection.ContainsId(item.Id);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public void When_object_does_not_exists_object_should_not_exist()
        {
            var item = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };

            var collection = new StreamCollection();

            // Act
            var exists = collection.ContainsId(item.Id);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public void When_objects_exists_return_dynamic()
        {
            var item = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };

            var collection = new StreamCollection();

            // Act
            collection.Added(item.Id, item);

            // Assert
            var result = collection.GetDynamicById(item.Id);

            ((string) result.Id).Should().Be(item.Id);
        }

        [Fact]
        public void When_objects_not_exists_return_dynamic()
        {
            var collection = new StreamCollection();

            // Act

            // Assert
            var result = collection.GetDynamicById(_autoFixture.Create<string>());

            ((object) result).Should().BeNull();
        }

        [Fact]
        public void When_name_is_set_store_it()
        {
            var collection = new StreamCollection();
            var name = _autoFixture.Create<string>();

            // Act
            collection.Name = name;

            // Assert
            collection.Name.Should().Be(name);
        }

        [Fact]
        public void When_object_is_added_and_collection_is_contains_id_override_new_object()
        {
            var existing = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };

            var item = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };

            var collection = new StreamCollection();

            // Act
            collection.Added(existing.Id, existing);
            collection.Added(existing.Id, item);

            // Assert
            var result = collection.GetAnonymousTypeById(existing.Id, item);

            result.Should().Be(item);
        }

        [Fact]
        public void When_object_is_changed_merge_existing_values()
        {
            var item = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };

            var other = new
            {
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                },
                NewValue = _autoFixture.Create<string>()
            };

            var collection = new StreamCollection();

            // Act
            collection.Added(item.Id, item);
            collection.Changed(item.Id, other);

            // Assert
            var result = collection.GetAnonymousTypeById(item.Id, item);

            result.Int.Should().Be(other.Int);
            result.Str.Should().Be(other.Str);
            result.Obj.Should().Be(other.Obj);

            result.Id.Should().Be(item.Id);
        }

        [Fact]
        public void When_an_empty_object_is_given_for_change_update_nothing()
        {
            var item = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };

            var other = new
            {
            };

            var collection = new StreamCollection();

            // Act
            collection.Added(item.Id, item);
            collection.Changed(item.Id, other);

            // Assert
            var result = collection.GetAnonymousTypeById(item.Id, item);

            result.Should().Be(item);
        }

        [Fact]
        public void When_object_doesnt_exist_return_null()
        {
            var collection = new StreamCollection();

            // Act

            // Assert
            var result = collection.GetById<object>(_autoFixture.Create<string>());

            result.Should().BeNull();
        }

        [Fact]
        public void When_object_deleted_remove_from_collection()
        {
            var item = new
            {
                Id = _autoFixture.Create<string>(),
                Str = _autoFixture.Create<string>(),
                Int = _autoFixture.Create<int>(),
                Obj = new
                {
                    Value = _autoFixture.Create<string>()
                }
            };
            var collection = new StreamCollection();

            // Act
            collection.Added(item.Id, item);
            collection.Removed(item.Id);

            // Assert
            var result = collection.GetAnonymousTypeById(item.Id, item);

            result.Should().BeNull();
        }
    }
}