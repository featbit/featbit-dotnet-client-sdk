using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeatBit.ClientSdk.Evaluation;
using FeatBit.ClientSdk.Model;

namespace FeatBit.ClientSdk
{
    public interface IFbClient : IDisposable
    {
        /// <summary>
        /// Indicates whether the client is ready to be used.
        /// </summary>
        /// <value>true if the client is ready</value>
        bool Initialized { get; }

        /// <summary>
        /// Changes the current evaluation use and requests flags for that user if we are online.
        /// </summary>
        Task IdentifyAsync(FbUser user);

        /// <summary>
        /// Get the boolean value of a feature flag for current user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the flag variation does not have a boolean value, <c>defaultValue</c> is returned.
        /// </para>
        /// <para>
        /// If an error makes it impossible to evaluate the flag (for instance, the feature flag key
        /// does not match any existing flag), <c>defaultValue</c> is returned.
        /// </para>
        /// </remarks>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>the variation for the given user, or <c>defaultValue</c> if the flag cannot be evaluated</returns>
        /// <seealso cref="BoolVariationDetail(string, bool)"/>
        bool BoolVariation(string key, bool defaultValue = false);

        /// <summary>
        /// Get the boolean value of a feature flag for a given user, and returns an object that
        /// describes the way the value was determined.
        /// </summary>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>an <see cref="EvalDetail{T}"/> object</returns>
        /// <seealso cref="BoolVariation(string, bool)"/>
        EvalDetail<bool> BoolVariationDetail(string key, bool defaultValue = false);

        /// <summary>
        /// Get the integer value of a feature flag for current user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the flag variation does not have a integer value, <c>defaultValue</c> is returned.
        /// </para>
        /// <para>
        /// If an error makes it impossible to evaluate the flag (for instance, the feature flag key
        /// does not match any existing flag), <c>defaultValue</c> is returned.
        /// </para>
        /// </remarks>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>the variation for the given user, or <c>defaultValue</c> if the flag cannot be evaluated</returns>
        /// <seealso cref="IntVariationDetail(string, int)"/>
        int IntVariation(string key, int defaultValue = 0);

        /// <summary>
        /// Get the integer value of a feature flag for a given user, and returns an object that
        /// describes the way the value was determined.
        /// </summary>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>an <see cref="EvalDetail{T}"/> object</returns>
        /// <seealso cref="IntVariation(string, int)"/>
        EvalDetail<int> IntVariationDetail(string key, int defaultValue = 0);


        /// <summary>
        /// Get the float value of a feature flag for current user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the flag variation does not have a float value, <c>defaultValue</c> is returned.
        /// </para>
        /// <para>
        /// If an error makes it impossible to evaluate the flag (for instance, the feature flag key
        /// does not match any existing flag), <c>defaultValue</c> is returned.
        /// </para>
        /// </remarks>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>the variation for the given user, or <c>defaultValue</c> if the flag cannot be evaluated</returns>
        /// <seealso cref="FloatVariationDetail(string, float)"/>
        /// <seealso cref="DoubleVariation(string, double)"/>
        float FloatVariation(string key, float defaultValue = 0);

        /// <summary>
        /// Get the float value of a feature flag for a given user, and returns an object that
        /// describes the way the value was determined.
        /// </summary>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>an <see cref="EvalDetail{T}"/> object</returns>
        /// <seealso cref="FloatVariation(string, float)"/>
        /// <seealso cref="DoubleVariationDetail(string, double)"/>
        EvalDetail<float> FloatVariationDetail(string key, float defaultValue = 0);

        /// <summary>
        /// Get the double value of a feature flag for current user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the flag variation does not have a double value, <c>defaultValue</c> is returned.
        /// </para>
        /// <para>
        /// If an error makes it impossible to evaluate the flag (for instance, the feature flag key
        /// does not match any existing flag), <c>defaultValue</c> is returned.
        /// </para>
        /// </remarks>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>the variation for the given user, or <c>defaultValue</c> if the flag cannot be evaluated</returns>
        /// <seealso cref="DoubleVariationDetail(string, double)"/>
        /// <seealso cref="FloatVariation(string, float)"/>
        double DoubleVariation(string key, double defaultValue = 0);

        /// <summary>
        /// Get the double value of a feature flag for a given user, and returns an object that
        /// describes the way the value was determined.
        /// </summary>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>an <see cref="EvalDetail{T}"/> object</returns>
        /// <seealso cref="DoubleVariation(string, double)"/>
        /// <seealso cref="FloatVariationDetail(string, float)"/>
        EvalDetail<double> DoubleVariationDetail(string key, double defaultValue = 0);

        /// <summary>
        /// Get the string value of a feature flag for current user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Normally, the string value of a flag should not be null, since the FeatBit UI
        /// does not allow you to assign a null value to a flag variation. However, since
        /// <c>defaultValue</c> is nullable, you should assume that the return value might be null.
        /// </para>
        /// </remarks>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>the variation for the given user, or <c>defaultValue</c> if the flag cannot
        /// be evaluated</returns>
        /// <seealso cref="StringVariationDetail(string, string)"/>
        string StringVariation(string key, string defaultValue = "");

        /// <summary>
        /// Get the string value of a feature flag for a given context, and returns an object that
        /// describes the way the value was determined.
        /// </summary>
        /// <param name="key">the unique feature key for the feature flag</param>
        /// <param name="defaultValue">the default value of the flag</param>
        /// <returns>an <see cref="EvalDetail{T}"/> object</returns>
        /// <seealso cref="StringVariation(string, string)"/>
        EvalDetail<string> StringVariationDetail(string key, string defaultValue = "");

        /// <summary>
        /// Get a map from feature flag keys to feature flag values for the current user.
        /// </summary>
        IDictionary<string, FeatureFlag> AllFlags();
    }
}