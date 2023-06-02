import { spotifyApiClient } from "../client";
import { ArtistsResponse } from "./types";

export const searchArtist = async (q: string) =>
  await spotifyApiClient.get<ArtistsResponse>(
    `${process.env.NEXT_PUBLIC_SPOTIFY_API_BASE_URL}/search?q=${q}&type=artist&limit=10`
  );