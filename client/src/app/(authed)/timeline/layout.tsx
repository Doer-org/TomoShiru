"use client";
import React, { useState } from "react";
import { Logo, Hamburger } from "@/ui";
import { CommonHeader, NavMenu } from "@/app/_ui";
import * as styles from "../_styles/common.css";

export default function CommmonLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const [open, setOpen] = useState<boolean>(false);

  const handleClick = () => {
    setOpen((prev) => !prev);
  };

  const navClassList = [styles.navStyle, open && styles.navOpenStyle];

  return (
    <>
      <CommonHeader
        title="タイムライン"
        left={<Logo />}
        right={<Hamburger onClick={handleClick} />}
      />
      <div className={navClassList.join(" ")}>
        <NavMenu />
      </div>
      <div className={styles.containerStyle}>
        <div className={styles.contentStyle}>{children}</div>
      </div>
    </>
  );
}