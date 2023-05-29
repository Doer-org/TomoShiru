import type { Meta, StoryObj } from "@storybook/react";

import { CommonHeader } from "../components";
import { Hamburger, Logo } from "@/ui";

const meta: Meta<typeof CommonHeader> = {
  title: "Hack/Header/CommonHeader",
  component: CommonHeader,
  decorators: [
    (Story) => (
      <div
        style={{
          height: "100vh",
          background: "#f9f8f4",
        }}
      >
        <Story />
      </div>
    ),
  ],
};

export default meta;
type Story = StoryObj<typeof CommonHeader>;

export const Timeline: Story = {
  args: {
    title: "タイムライン",
    left: <Logo />,
    right: <Hamburger />,
  },
};
