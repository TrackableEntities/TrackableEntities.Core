using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    public static class TestHelpers
    {
        public static void AssertStates(List<EntityState> states, EntityState expectedState)
        {
            var asserts = new Action<EntityState>[40];
            for (var i = 0; i < asserts.Length; i++)
                asserts[i] = state => Assert.Equal(expectedState, state);
            Assert.Collection(states, asserts);
        }
    }
}
