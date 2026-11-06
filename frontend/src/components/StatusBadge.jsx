import { label } from '../constants';

export default function StatusBadge({ status }) {
  return <span className={`badge badge-${status.toLowerCase()}`}>{label(status)}</span>;
}
