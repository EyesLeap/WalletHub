import React from "react";
import { Link } from "react-router-dom";

interface Props {}

const Hero = (props: Props) => {
  return (
    <section
      id="hero"
      className="bg-[var(--color-bg)] text-[var(--color-text)] py-32 flex flex-col items-center text-center px-6"
    >
      <h1 className="text-5xl lg:text-6xl font-bold max-w-4xl leading-tight mb-6">
        Manage your crypto wallets<br /> in one convenient place.
      </h1>
      <p className="text-xl text-[var(--color-text-muted)] max-w-1xl mb-10">
        Track portfolios, monitor transactions, and stay in control — all from one dashboard.
      </p>
      <Link
        to="/dashboard"
        className="px-10 py-5 text-xl font-semibold rounded bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] transition duration-200"
      >
        Start Now
      </Link>
    </section>
  );
};

export default Hero;
