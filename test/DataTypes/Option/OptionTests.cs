﻿using FluentAssertions;
using NUnit.Framework;
using TinyFp;
using TinyFp.Extensions;

namespace TinyFpTest.DataTypes
{
    [TestFixture]
    public class OptionTests
    {
        [Test]
        public void None_CreateNoneObject()
            => Option<string>
                    .None()
                    .IsNone
                .Should().BeTrue();

        [Test]
        public void Some_CreateSomeObject()
            => Option<string>
                    .Some("test")
                    .IsSome
                .Should().BeTrue();

        [Test]
        public void OnNone_CallTheAction()
        {
            var called = false;
            _ = Option<string>
                .None()
                .OnNone(() => called = true);

            called.Should().BeTrue();
        }

        [Test]
        public void OnNone_WhenSome_DontCallTheAction()
        {
            var called = false;
            _ = Option<string>
                .Some("not-empty")
                .OnNone(() => called = true);

            called.Should().BeFalse();
        }

        [Test]
        public void OnNone_CallTheFunction()
            => Option<string>
                    .None()
                    .OrElse(() => "empty")
                .Should().Be("empty");

        [Test]
        public void OnNone_WhenSome_DontCallTheFunction()
            => Option<string>
                    .Some("not-empty")
                    .OrElse(() => "empty")
                .Should().Be("not-empty");

        [Test]
        public void OnNone_ReturnSomeObject()
            => Option<string>
                    .None()
                    .OrElse("empty")
                .Should().Be("empty");

        [Test]
        public void OnNone_WhenSome_ReturnSome()
            => Option<string>
                    .Some("not-empty")
                    .OrElse("empty")
                .Should().Be("not-empty");

        [Test]
        public void OnSome_CallTheAction()
        {
            var called = false;
            _ = Option<string>
                .Some("not-empty")
                .OnSome(_ => called = _ == "not-empty");

            called.Should().BeTrue();
        }

        [Test]
        public void Map_MapInputInOutput()
            => Option<string>
                    .Some("not-empty")
                    .Map(_ => _ == "not-empty")
                    .OrElse(false)
                .Should().BeTrue();

        [Test]
        public void UnWrap_WhenIsSome_ShouldReturnValue()
            => Option<string>
                .Some("not-empty")
                .Unwrap()
                .Should()
                .Be("not-empty");

        [Test]
        public void UnWrap_WhenIsNone_ShouldThrow()
            => Option<string>
                .None()
                .Invoking(_ => _.Unwrap())
                .Should()
                .Throw<InvalidOperationException>();

        [Test]
        public void Map_MapNoneInOutput()
            => Option<string>
                    .None()
                    .Map(_ => true)
                    .IsNone
                .Should().BeTrue();

        [Test]
        public void MapAsync_MapInputInOutput()
            => Option<string>
                    .Some("not-empty")
                    .MapAsync(_ => Task.FromResult(_ == "not-empty"))
                    .Result
                    .OrElse(false)
                .Should().BeTrue();

        [Test]
        public void MapAsync_MapNoneInOutput()
            => Option<string>
                    .None()
                    .MapAsync(_ => Task.FromResult(true))
                    .Result
                    .IsNone
                .Should().BeTrue();

        [Test]
        public void Bind_MapInputInOutput()
            => Option<string>
                    .Some("not-empty")
                    .Bind(_ => (_ == "not-empty").ToOption(__ => !__))
                    .IsSome
                .Should().BeTrue();

        [Test]
        public void Bind_MapNoneInOutput()
            => Option<string>
                    .None()
                    .Bind(_ => Option<bool>.Some(true))
                    .IsNone
                .Should().BeTrue();

        [Test]
        public void BindAsync_MapInputInOutput()
            => Option<string>
                    .Some("not-empty")
                    .BindAsync(_ => Task.FromResult(Option<bool>.Some(_ == "not-empty")))
                    .Result
                    .OrElse(false)
                .Should().BeTrue();

        [Test]
        public void BindAsync_MapNoneInOutput()
            => Option<string>
                    .None()
                    .BindAsync(_ => Task.FromResult(Option<bool>.Some(true)))
                    .Result
                    .IsNone
                .Should().BeTrue();

        [Test]
        public void Match_IfSome_ToOutput()
            => Option<string>
                    .Some("not-empty")
                    .Match(_ => _ == "not-empty",
                           () => false )                    
                .Should().BeTrue();

        [Test]
        public void Match_IfNone_ToOutput()
            => Option<string>
                    .None()
                    .Match(_ => false,
                           () => true)
                .Should().BeTrue();

        [Test]
        public void MatchAsync_IfSome_ToOutput()
            => Option<string>
                    .Some("not-empty")
                    .MatchAsync(_ => Task.FromResult(_ == "not-empty"),
                           () => Task.FromResult(false))
                    .Result
                .Should().BeTrue();

        [Test]
        public void MatchAsync_IfNone_ToOutput()
            => Option<string>
                    .None()
                    .MatchAsync(_ => Task.FromResult(false),
                           () => Task.FromResult(true))
                    .Result
                .Should().BeTrue();

        [Test]
        public void MatchAsync_1_IfSome_ToOutput()
            => Option<string>
                    .Some("not-empty")
                    .MatchAsync(_ => _ == "not-empty",
                           () => Task.FromResult(false))
                    .Result
                .Should().BeTrue();

        [Test]
        public void MatchAsync_1_IfNone_ToOutput()
            => Option<string>
                    .None()
                    .MatchAsync(_ => false,
                           () => Task.FromResult(true))
                    .Result
                .Should().BeTrue();

        [Test]
        public void MatchAsync_2_IfSome_ToOutput()
            => Option<string>
                    .Some("not-empty")
                    .MatchAsync(_ => Task.FromResult(_ == "not-empty"),
                           () => false)
                    .Result
                .Should().BeTrue();

        [Test]
        public void MatchAsync_2_IfNone_ToOutput()
            => Option<string>
                    .None()
                    .MatchAsync(_ => Task.FromResult(false),
                           () => true)
                    .Result
                .Should().BeTrue();

        [Test]
        public void ToEither_Func_WhenSome_Right()
            => Option<string>
                    .Some("not-empty")
                    .ToEither(() => 0)
                .IsRight
                .Should().BeTrue();

        [Test]
        public void ToEither_Func_WhenNone_Left()
            => Option<string>
                    .None()
                    .ToEither(() => 0)
                .IsLeft
                .Should().BeTrue();

        [Test]
        public void ToEither_WhenSome_Right()
            => Option<string>
                    .Some("not-empty")
                    .ToEither(0)
                .IsRight
                .Should().BeTrue();

        [Test]
        public void ToEither_WhenNone_Left()
            => Option<string>
                    .None()
                    .ToEither(0)
                .IsLeft
                .Should().BeTrue();

        [Test]
        public void OperatorTrue_WhenSome_IsTrue()
        {
            if (Option<string>.Some("some"))
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void OperatorTrue_WhenNone_IsFalse()
        {
            if (Option<string>.None())
                Assert.Fail();
            else
                Assert.Pass();
        }

        [Test]
        public void OperatorNot_WhenSome_IsFalse()
        {
            if (!Option<string>.Some("some"))
                Assert.Fail();
            else
                Assert.Pass();
        }

        [Test]
        public void OperatorNot_WhenNone_IsTrue()
        {
            if (!Option<string>.None())
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void Match_IfSome_CallActionSome()
        {
            var sideEffect = 0;
            Option<string>
                    .Some("not-empty")
                    .Match(_ => sideEffect = 1,
                           () => { });
            sideEffect.Should().Be(1);
        }

        [Test]
        public void Match_IfNone_CallActionNone()
        {
            var sideEffect = 0;
            Option<string>
                    .None()
                    .Match(_ => { },
                           () => sideEffect = 1);
            sideEffect.Should().Be(1);
        }

        [Test]
        public void MapNone_MapInputInOutput()
            => Option<string>
                    .Some("not-empty")
                    .MapNone(() => "empty")
                .Match(
                    _ => _.Should().Be("not-empty"),
                    () => Assert.Fail());

        [Test]
        public void MapNone_MapNoneInOutput()
            => Option<string>
                    .None()
                    .MapNone(() => "empty")
                .Match(
                    _ => _.Should().Be("empty"),
                    () => Assert.Fail());

        [Test]
        public void MapNoneAsync_MapInputInOutput()
            => Option<string>
                    .Some("not-empty")
                    .MapNoneAsync(() => Task.FromResult("empty"))
                .Result
                .Match(
                    _ => _.Should().Be("not-empty"),
                    () => Assert.Fail());

        [Test]
        public void MapNoneAsync_MapNoneInOutput()
            => Option<string>
                    .None()
                    .MapNoneAsync(() => Task.FromResult("empty"))
                .Result
                .Match(
                    _ => _.Should().Be("empty"),
                    () => Assert.Fail());

        [Test]
        public void BindNone_MapInputInOutput()
            => Option<string>
                    .Some("not-empty")
                    .BindNone(() => Option<string>.Some("empty"))
                .Match(
                    _ => _.Should().Be("not-empty"),
                    () => Assert.Fail());

        [Test]
        public void BindNone_MapNoneInOutput()
            => Option<string>
                    .None()
                    .BindNone(() => Option<string>.Some("empty"))
                .Match(
                    _ => _.Should().Be("empty"),
                    () => Assert.Fail());

        [Test]
        public void BindNoneAsync_MapInputInOutput()
            => Option<string>
                    .Some("not-empty")
                    .BindNoneAsync(() => Task.FromResult(Option<string>.Some("empty")))
                .Result
                .Match(
                    _ => _.Should().Be("not-empty"),
                    () => Assert.Fail());

        [Test]
        public void BindNoneAsync_MapNoneInOutput()
            => Option<string>
                    .None()
                    .BindNoneAsync(() => Task.FromResult(Option<string>.Some("empty")))
                .Result
                .Match(
                    _ => _.Should().Be("empty"),
                    () => Assert.Fail());

        [Test]
        public void Guard_Map_WhenDefault()
        {
            Option<int>.Some(4)
                .GuardMap(
                    _ => _ / 2,
                    (_ => _ % 2 == 1, _ => _ + 1))
                .Match(_ => _, () => int.MinValue)
                .Should()
                .Be(2);
        }

        [Test]
        public void Guard_Map_WhenMatchesFirst()
        {
            Option<int>.Some(5)
                .GuardMap(
                    _ => _ / 2,
                    (_ => _ % 2 == 1, _ => _ + 1))
                .Match(_ => _, () => int.MinValue)
                .Should()
                .Be(6);
        }

        [Test]
        public void Guard_Bind_WhenDefault()
        {
            Option<int>.Some(4)
                .GuardBind(
                    _ => Option<int>.Some(_ / 2),
                    (_ => _ % 2 == 1, _ => Option<int>.None()))
                .Match(_ => _, () => int.MinValue)
                .Should()
                .Be(2);
        }

        [Test]
        public void Guard_Bind_WhenMatchesFirst()
        {
            Option<int>.Some(5)
                .GuardBind(
                    _ => Option<int>.Some(_ / 2),
                    (_ => _ % 2 == 1, _ => Option<int>.None()))
                .Match(_ => _, () => int.MinValue)
                .Should()
                .Be(int.MinValue);
        }

        [Test]
        public void GuardMapAsync_WhenMatches()
        {
            Option<int>.Some(5)
                .GuardMapAsync(
                async value => await Task.Run(() => value - 5),
                (_ => _ == 5, _ => Task.FromResult(_ * 2)))
                .Result
                .Match(_ => _, () => int.MinValue)
                .Should()
                .Be(10);
        }

        [Test]
        public void GuardBindAsync_WhenMatches()
        {
            Option<int>.Some(5)
                .GuardBindAsync(
                    value => Task.FromResult(Option<string>.Some((value / 2).ToString())),
                    (
                        value => value % 2 == 1,
                        value => Option<int>.Some(value)
                                    .GuardMapAsync(
                                        _ => Task.FromResult((_ + 1).ToString()),
                                        (_ => _ < 10, _ => Task.FromResult(((_ + 1) * 2).ToString())
                                    )
                    )
                ))
                .Result
                .OnNone(() => Assert.Fail())
                .OnSome(_ => _.Should().Be("12"));
        }
    }
}
