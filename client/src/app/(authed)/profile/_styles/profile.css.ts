import { style } from "@vanilla-extract/css";

export const linkStyle = style({
  textDecoration: "none",
  color: "#fff",
});

export const headStyle = style({
  display: "flex",
  justifyContent: "space-between",
  alignItems: "center",
});

export const categoriesStyle = style({
  display: "flex",
  gap: "10px",
});

export const cardListStyle = style({
  marginTop: "24px",
});

export const cardStyle = style({
  selectors: {
    "&:not(:first-child)": {
      marginTop: "24px",
    },
  },
});

export const formWrapperStyle = style({
  marginTop: "18px",
});

export const resultListStyle = style({
  marginTop: "18px",
});

export const artistStyle = style({
  selectors: {
    "&:not(:first-child)": {
      marginTop: "12px",
    },
  },
});
