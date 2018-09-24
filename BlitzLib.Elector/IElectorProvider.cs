namespace BlitzLib.Elector
{
    /// <summary>
    /// Contract to implement an elector provider
    /// </summary>
    public interface IElectorProvider
    {
        /// <summary>
        /// Am I the Primary?
        /// <para>Also causes the renewal of the lease for this instance if it is primary</para>
        /// <para>If the primary has expired, awards Primary to 1st instance to call this method next</para>
        /// </summary>
        /// <param name="info">Elector Info</param>
        bool AmIPrimary(Models.ElectorInfo info);

        /// <summary>
        /// Force an election
        /// </summary>
        /// <param name="applicationName">Application name</param>
        /// <returns>True if election is forced</returns>
        bool ForceElection(string applicationName);

        /// <summary>
        /// How long in <c>milliseconds</c> the primary has to act and call <c>AmIPrimary</c> again
        /// </summary>
        /// <param name="milliseconds">Primary expires after milliseconds of non-activity</param>
        void SetExpirationMilliseconds(int milliseconds);

        /// <summary>
        /// Returns expiration duration in milliseconds
        /// </summary>
        /// <returns>milliseconds</returns>
        int GetExpirationMilliseconds();
    }

}

