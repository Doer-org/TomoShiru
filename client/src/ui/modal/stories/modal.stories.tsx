import type { Meta, StoryObj } from "@storybook/react";

import { Modal } from "../components/modal";

const meta: Meta<typeof Modal> = {
  title: "Hack/Modal",
  component: Modal,
  decorators: [
    (Story) => (
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100vh",
          background: "#00000066",
        }}
      >
        <Story />
      </div>
    ),
  ],
};

export default meta;
type Story = StoryObj<typeof Modal>;

export const Default: Story = {
  render: () => {
    return <Modal>Default</Modal>;
  },
};
