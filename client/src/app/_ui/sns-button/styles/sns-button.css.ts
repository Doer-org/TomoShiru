import { style } from "@vanilla-extract/css";

export const contentStyle = style({
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  gap: "10px",
});

export const iconWrapperStyle = style({
  width: "32px",
  height: "32px",
});

export const labelStyle = style({
  fontSize: "14px",
});
