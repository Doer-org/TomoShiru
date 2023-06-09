import { style, styleVariants } from "@vanilla-extract/css";

export const wrapperStyle = style({
  borderRadius: "50%",
  cursor: "pointer",
  flexShrink: "0",
  background: "#f9f8f4",
});

export const imageStyle = style({
  width: "100%",
  height: "100%",
  borderRadius: "50%",
});

export const size = styleVariants({
  tiny: { width: "28px", height: "28px" },
  small: { width: "42px", height: "42px" },
  medium: { width: "52px", height: "52px" },
  large: { width: "62px", height: "62px" },
});
