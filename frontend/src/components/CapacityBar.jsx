import { CAPACITY_COLORS, capacityState } from '../constants';

export default function CapacityBar({ active, max }) {
  const state = capacityState(active, max);
  const percent = max === 0 ? 0 : Math.min(100, (active / max) * 100);

  return (
    <div className="capacity" title={`${active} of ${max} slots leased`}>
      <div className="capacity-track">
        <div className="capacity-fill" style={{ width: `${percent}%`, background: CAPACITY_COLORS[state.key] }} />
      </div>
      <span className="capacity-text">
        {active}/{max} · {state.text}
      </span>
    </div>
  );
}
