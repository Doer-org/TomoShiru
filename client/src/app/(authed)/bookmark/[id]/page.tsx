"use client";
import { getReaction, getUser } from "@/api";
import { CommonHeader } from "@/app/_ui";
import { Arrow, Avatar, Card, Like, Logo } from "@/ui";
import { User } from "@/utils";
import React, { useEffect, useState } from "react";
import * as commonStyles from "../../_styles/common.css";
import * as styles from "../_styles/bookmark.css";

type Props = {
  params: { id: string };
};

const guest: User extends { data: infer U } ? U : never = {
  user_id: "guest",
  user_name: "Guest",
  image_url: "",
};

const Page = ({ params }: Props) => {
  const [bookmarkDOM, setBookmarkDOM] = useState<React.ReactNode[]>();

  useEffect(() => {
    (async () => {
      const reactions = await getReaction(params.id);
      const bookmarks = reactions?.filter((r) => r.user_id_from === params.id);
      const bDOM: React.ReactNode[] = [];
      bookmarks?.forEach(async (b) => {
        const user = await getUser(b.user_id_to);
        const tmpReactions = await getReaction(user?.user_id || "abc");
        const tmpBookmarks = reactions?.filter(
          (r) => r.user_id_to === user?.user_id
        );
        bDOM.push(
          <div
            key={b.user_id_to + b.timestamp}
            className={styles.cardWrapperStyle}
          >
            <Card>
              <div className={styles.cardInnerStyle}>
                <div className={styles.cardUserStyle}>
                  <Avatar size="small" image={user?.image_url} />
                  <div>
                    <span className={styles.cardUserNameStyle}>
                      {user?.user_name}
                    </span>
                    さんの音楽プロフィール
                  </div>
                </div>
                <Like
                  user={user || guest}
                  num={tmpBookmarks?.length || 0}
                  liked={true}
                />
              </div>
            </Card>
          </div>
        );
        setBookmarkDOM(bDOM);
      });
    })();
  }, []);

  return (
    <>
      <CommonHeader
        title="いいねしたプロフィール"
        left={<Arrow />}
        right={<Logo />}
      />
      <div
        className={[
          commonStyles.containerStyle,
          commonStyles.headerAvoidStyle["common"],
        ].join(" ")}
      >
        <div className={commonStyles.contentStyle}>{bookmarkDOM}</div>
      </div>
    </>
  );
};

export default Page;
