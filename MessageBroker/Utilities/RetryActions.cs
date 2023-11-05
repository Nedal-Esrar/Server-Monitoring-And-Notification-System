namespace MessageBroker.Utilities;

public static class RetryActions
{
  // delayBetweenRetries in ms
  public static TItem PerformWithRetry<TItem>(int maxRetryCount, int delayBetweenRetries, Func<TItem> action)
  {
    Exception toThrowIfNowSuccessful = null;
    
    for (var retryCount = 0; retryCount < maxRetryCount; ++retryCount)
    {
      try
      {
        var ret = action();

        return ret;
      }
      catch (Exception ex)
      {
        toThrowIfNowSuccessful = ex;
      }

      Thread.Sleep(delayBetweenRetries);
    }

    throw toThrowIfNowSuccessful!;
  }
}