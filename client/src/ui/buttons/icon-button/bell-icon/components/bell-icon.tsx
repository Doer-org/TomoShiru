import React from "react";
import * as styles from "../styles/bell-icon.css";

type Props = {
  fill?: boolean;
  onClick?: () => void;
};

const _BellIcon = ({ fill = false, ...props }: Props) => {
  return (
    <div className={styles.wrapperStyle} {...props}>
      <img
        className={styles.imageStyle}
        src={fill ? "/assets/bell-fill.svg" : "/assets/bell-line.svg"}
        alt="ベル"
      />
    </div>
  );
};

export const BellIcon = _BellIcon;
