import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
} from 'recharts';
import { useEffect, useState } from 'react';
import { PortfolioSnapshotGet, PortfolioSnapshotRange } from '../../Models/PortfolioSnapshot';
import moment from 'moment';
import styles from './PortfolioChart.module.css';
import { formatLargeMonetaryNumber } from '../../Helpers/NumberFormatting';


interface Props {
  snapshots: PortfolioSnapshotGet[];
  latestPortfolioValue: number | undefined;
}

const PortfolioChart = ({ snapshots, latestPortfolioValue }: Props) => {
  const [filter, setFilter] = useState<PortfolioSnapshotRange>(PortfolioSnapshotRange.Last24Hours);
  const [filteredData, setFilteredData] = useState<PortfolioSnapshotGet[]>([]);
  const [ticks, setTicks] = useState<number[]>([]);
  const [snapshotDots, setSnapshotDots] = useState<Record<number, boolean>>({});

  const generateFilledData = (snapshots: PortfolioSnapshotGet[], range: PortfolioSnapshotRange) => {
    const now = new Date();
    const end = new Date(now);
    end.setMinutes(0, 0, 0);
    let start = new Date();

    const allTimes = snapshots.map(s => moment.utc(s.createdAt).local().toDate().getTime());
    const minTime = Math.min(...allTimes);

    switch (range) {
      case PortfolioSnapshotRange.Last24Hours:
        start = new Date(end.getTime() - 24 * 60 * 60 * 1000);
        break;
      case PortfolioSnapshotRange.Last7Days:
        start = new Date(Math.max(minTime, now.getTime() - 7 * 24 * 60 * 60 * 1000));
        start.setHours(8, 0, 0, 0);
        break;
      case PortfolioSnapshotRange.Last30Days:
        start = new Date(Math.max(minTime, now.getTime() - 30 * 24 * 60 * 60 * 1000));
        start.setHours(1, 0, 0, 0);
        break;
      case PortfolioSnapshotRange.All:
        start = new Date(minTime);  
        start.setHours(1, 0, 0, 0);
        break;
      default:
        start = new Date(now);
        break;
    }

    const tickTimes: number[] = [];

    const sortedSnapshots = [...snapshots]
      .map(s => ({
        ...s,
        time: moment.utc(s.createdAt).local().toDate().getTime(),
      }))
      .sort((a, b) => a.time - b.time).reverse();

    let lastValue = sortedSnapshots.length > 0 ? sortedSnapshots[0].totalValueUSD : 0;
    const dotsMap: Record<number, boolean> = {};
    const filled: any[] = [];

    sortedSnapshots.forEach(s => {
      dotsMap[s.time] = true;
    });

    if (range === PortfolioSnapshotRange.Last24Hours) {
      for (let i = 1; i < 24; i += 3) {
        tickTimes.push(start.getTime() + i * 60 * 60 * 1000);
      }
      for (let i = 1; i <= 24; i++) {
        const current = start.getTime() + i * 60 * 60 * 1000;
        const snapshot = sortedSnapshots.find(s => s.time <= current);
        if (snapshot) {
          lastValue = snapshot.totalValueUSD;
        }

        filled.push({
          time: current,
          USD: lastValue,
          isSnapshot: !!dotsMap[current],
        });
      }
    } else if (range === PortfolioSnapshotRange.Last7Days) {
      const startTime = start.getTime();
      const endTime = now.getTime();
      const totalHours = Math.ceil((endTime - startTime) / (60 * 60 * 1000));
      const totalDays = Math.ceil((endTime - startTime) / (24 * 60 * 60 * 1000));

      for (let i = 0; i < totalHours; i += 6) {
        const current = startTime + i * 60 * 60 * 1000;
        const snapshot = sortedSnapshots.find(s => s.time <= current);
        if (snapshot) {
          lastValue = snapshot.totalValueUSD;
        }

        filled.push({
          time: current,
          USD: lastValue,
          isSnapshot: !!dotsMap[current],
        });
      }

      for (let i = 0; i < totalDays; i++) {
        const tick = startTime + i * 24 * 60 * 60 * 1000;
        tickTimes.push(tick);
      }
    } else if (range === PortfolioSnapshotRange.Last30Days) {
      const startTime = start.getTime();
      const endTime = now.getTime();
      const totalDays = Math.ceil((endTime - startTime) / (24 * 60 * 60 * 1000));

      let tickInterval = 3;
      if (totalDays <= 9) tickInterval = 1;
      else if (totalDays <= 20) tickInterval = 2;

      const totalActualDays = Math.ceil((endTime - startTime) / (24 * 60 * 60 * 1000));
      for (let i = 0; i < totalActualDays; i++) {
        const current = startTime + i * 24 * 60 * 60 * 1000;

        const snapshot = sortedSnapshots.find(s => s.time <= current);
        if (snapshot) {
          lastValue = snapshot.totalValueUSD;
        }

        filled.push({
          time: current,
          USD: lastValue,
          isSnapshot: !!dotsMap[current],
        });

        if (i % tickInterval === 0) {
          tickTimes.push(current);
        }
      }
    } else if (range === PortfolioSnapshotRange.All) {
      const startTime = start.getTime();
      const endTime = now.getTime();
      const totalDays = Math.ceil((endTime - startTime) / (24 * 60 * 60 * 1000));
      const tickInterval = Math.max(1, Math.floor(totalDays / 6));

      for (let i = 0; i < totalDays; i++) {
        const current = startTime + i * 24 * 60 * 60 * 1000;
        const snapshot = sortedSnapshots.find(s => s.time <= current);
        if (snapshot) {
          lastValue = snapshot.totalValueUSD;
        }

        filled.push({
          time: current,
          USD: lastValue,
          isSnapshot: !!dotsMap[current],
        });

        if (i % tickInterval === 0) {
          tickTimes.push(current);
        }
      }
    }

    if (latestPortfolioValue !== null) {
      filled.push({ 
        time: now.getTime(),
        USD: latestPortfolioValue,
        isSnapshot: false,
      });
    }

    return { data: filled, ticks: tickTimes, dotsMap };
  };

  useEffect(() => {
    const { data, ticks, dotsMap } = generateFilledData(snapshots, filter);
    setFilteredData(data);
    setTicks(ticks);
    setSnapshotDots(dotsMap);
  }, [snapshots, filter, latestPortfolioValue]);

  const filterButtons = [
    { label: '24H', value: PortfolioSnapshotRange.Last24Hours },
    { label: '7D', value: PortfolioSnapshotRange.Last7Days },
    { label: '30D', value: PortfolioSnapshotRange.Last30Days },
    { label: 'All', value: PortfolioSnapshotRange.All },
  ];

  return (
    <div className={styles.portfolioChartContainer}>
      <div className={styles.filterHeader}>
        <h2 className={styles.filterTitle}>History</h2>
        <div className={styles.filterButtons}>
          {filterButtons.map(btn => (
            <button
              key={btn.label}
              onClick={() => setFilter(btn.value)}
              className={`${styles.filterButton} ${filter === btn.value ? styles.selected : ''}`}
            >
              {btn.label}
            </button>
          ))}
        </div>
      </div>

      <ResponsiveContainer width="100%" height="80%">
        <AreaChart data={filteredData} margin={{ top: 10, right: 10, left: 10, bottom: 0 }}>
          <defs>
            <linearGradient id="colorValue" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#8884d8" stopOpacity={0.4} />
              <stop offset="95%" stopColor="#8884d8" stopOpacity={0} />
            </linearGradient>
          </defs>
          <CartesianGrid className={styles.chartGrid} />
          <XAxis
            stroke="#9ca3af"
            dataKey="time"
            type="number"
            domain={['dataMin', 'dataMax']}
            ticks={ticks}
            tickFormatter={(unixTime) => {
              switch (filter) {
                case PortfolioSnapshotRange.Last24Hours:
                  return moment(unixTime).format('HH:mm');
                case PortfolioSnapshotRange.Last7Days:
                case PortfolioSnapshotRange.Last30Days:
                case PortfolioSnapshotRange.All:
                  return moment(unixTime).format('DD MMM');
                default:
                  return '';
              }
            }}
            className={styles.chartAxis}
          />
          <YAxis
            stroke="#9ca3af"
            tickFormatter={(value) => `${formatLargeMonetaryNumber(value)}`}
            className={styles.chartAxis}
          />
          <Tooltip
            formatter={(value: number) => `${value.toFixed(2)}`}
            labelFormatter={(label) => moment(label).format('DD MMM, HH:mm')}
          />
          <Area
            type="linear"
            dataKey="USD"
            stroke="#8884d8"
            fillOpacity={1}
            fill="url(#colorValue)"
            dot={(props) => {
              const { cx, cy, payload } = props;
              if (!payload?.isSnapshot) return <g />;
              return (
                <circle
                  cx={cx}
                  cy={cy}
                  r={3}
                  className={styles.chartDot}
                />
              );
            }}
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
};

export default PortfolioChart;
