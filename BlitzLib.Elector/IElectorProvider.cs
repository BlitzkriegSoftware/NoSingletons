namespace BlitzLib.Elector
{
    /// <summary>
    /// Contract to implement an elector provider
    /// </summary>
    public interface IElectorProvider
    {
        /// <summary>
        /// Am I master? 
        /// </summary>
        /// <param name="info">Elector Info</param>
        bool AmIMaster(Models.ElectorInfo info);

        /// <summary>
        /// Expiration of Mastery if not updated
        /// </summary>
        /// <param name="milliseconds">Master expires after milliseconds of non-activity</param>
        void SetExpirationMilliseconds(int milliseconds);

        /// <summary>
        /// Returns expiration tolerance in milliseconds
        /// </summary>
        /// <returns>milliseconds</returns>
        int GetExpirationMilliseconds();
    }

}

