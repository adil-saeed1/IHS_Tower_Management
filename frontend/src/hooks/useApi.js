import { useEffect, useState } from 'react';

// Loads data whenever deps change; reload() refetches after a mutation.
export function useApi(loader, deps = []) {
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);
  const [version, setVersion] = useState(0);

  useEffect(() => {
    let ignore = false;
    setLoading(true);
    setError(null);

    loader()
      .then((result) => !ignore && setData(result))
      .catch((e) => !ignore && setError(e.message))
      .finally(() => !ignore && setLoading(false));

    // Ignore stale responses, e.g. while the user is still typing a search.
    return () => {
      ignore = true;
    };
  }, [...deps, version]);

  return { data, error, loading, reload: () => setVersion((v) => v + 1) };
}
