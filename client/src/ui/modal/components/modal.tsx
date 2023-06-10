import React from "react";
import * as styles from "../styles/modal.css";

type Props = {
  children: React.ReactNode;
};

const _Modal = ({ children, ...props }: Props) => {
  return (
    <div className={styles.frameStyle}>
      <div className={styles.crossWrapperStyle}>
        <div className={styles.crossStyle}>
          <span className={styles.crossBarStyle}></span>
          <span className={styles.crossBarStyle}></span>
        </div>
      </div>
      <div className={styles.contentStyle}>{children}</div>
    </div>
  );
};

export const Modal = _Modal;
