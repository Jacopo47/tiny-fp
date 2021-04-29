﻿using System;
using TinyFp;

namespace TinyFp.Extensions
{
    public static class FunctionalExtension
    {
        public static T TeeWhen<T>(this T @this, Func<T, T> tee, Func<bool> when)
            => when() ? tee(@this) : @this;

        public static T TeeWhen<T>(this T @this, Func<T, T> tee, Func<T, bool> when)
            => when(@this) ? tee(@this) : @this;

        public static T Tee<T>(this T @this, Func<T, T> tee)
            => tee(@this);

        public static T Tee<T>(this T @this, Action<T> tee)
            => @this.Tee(_ =>
            {
                tee(_);
                return _;
            });

        public static void Do<T>(this T @this, Action<T> action)
            => action(@this);

        public static Option<M> ToOption<A, M>(this A @this,
                                               Func<A, M> map,
                                               Predicate<A> noneWhen)
            => @this == null || noneWhen(@this) ?
                Option<M>.None() :
                Option<M>.Some(map(@this));

        public static Option<A> ToOption<A>(this A @this, Predicate<A> noneWhen)
            => ToOption(@this, _ => _, noneWhen);

        public static Option<A> ToOption<A>(this A @this)
            => ToOption(@this, _ => _, _ => false);
    }
}