import { style } from "@vanilla-extract/css";

export const containerStyle = style({
  paddingTop: "78px",
  paddingBottom: "56px",
});

export const contentStyle = style({
  margin: "24px 0",
  padding: "0 24px",
});

export const cardStyle = style({
  display: "flex",
  justifyContent: "space-between",
});

export const cardUserStyle = style({
  display: "flex",
  alignItems: "center",
  gap: "20px",
});