// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2015 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file is automatically generated.
// Changes to this file will be reverted when you update Steamworks.NET

using System;
using System.Runtime.InteropServices;

namespace Steamworks {
	public static class SteamMusicRemote {
		/// <summary>
		/// <para> Service Definition</para>
		/// </summary>
		public static bool RegisterSteamMusicRemote(string pchName) {
			InteropHelp.TestIfAvailableClient();
			using (var pchName2 = new InteropHelp.UTF8StringHandle(pchName)) {
				return NativeMethods.ISteamMusicRemote_RegisterSteamMusicRemote(pchName2);
			}
		}

		public static bool DeregisterSteamMusicRemote() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_DeregisterSteamMusicRemote();
		}

		public static bool BIsCurrentMusicRemote() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_BIsCurrentMusicRemote();
		}

		public static bool BActivationSuccess(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_BActivationSuccess(bValue);
		}

		public static bool SetDisplayName(string pchDisplayName) {
			InteropHelp.TestIfAvailableClient();
			using (var pchDisplayName2 = new InteropHelp.UTF8StringHandle(pchDisplayName)) {
				return NativeMethods.ISteamMusicRemote_SetDisplayName(pchDisplayName2);
			}
		}

		public static bool SetPNGIcon_64x64(byte[] pvBuffer, uint cbBufferLength) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_SetPNGIcon_64x64(pvBuffer, cbBufferLength);
		}

		/// <summary>
		/// <para> Abilities for the user interface</para>
		/// </summary>
		public static bool EnablePlayPrevious(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_EnablePlayPrevious(bValue);
		}

		public static bool EnablePlayNext(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_EnablePlayNext(bValue);
		}

		public static bool EnableShuffled(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_EnableShuffled(bValue);
		}

		public static bool EnableLooped(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_EnableLooped(bValue);
		}

		public static bool EnableQueue(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_EnableQueue(bValue);
		}

		public static bool EnablePlaylists(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_EnablePlaylists(bValue);
		}

		/// <summary>
		/// <para> Status</para>
		/// </summary>
		public static bool UpdatePlaybackStatus(AudioPlayback_Status nStatus) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_UpdatePlaybackStatus(nStatus);
		}

		public static bool UpdateShuffled(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_UpdateShuffled(bValue);
		}

		public static bool UpdateLooped(bool bValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_UpdateLooped(bValue);
		}

		/// <summary>
		/// <para> volume is between 0.0 and 1.0</para>
		/// </summary>
		public static bool UpdateVolume(float flValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_UpdateVolume(flValue);
		}

		/// <summary>
		/// <para> Current Entry</para>
		/// </summary>
		public static bool CurrentEntryWillChange() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_CurrentEntryWillChange();
		}

		public static bool CurrentEntryIsAvailable(bool bAvailable) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_CurrentEntryIsAvailable(bAvailable);
		}

		public static bool UpdateCurrentEntryText(string pchText) {
			InteropHelp.TestIfAvailableClient();
			using (var pchText2 = new InteropHelp.UTF8StringHandle(pchText)) {
				return NativeMethods.ISteamMusicRemote_UpdateCurrentEntryText(pchText2);
			}
		}

		public static bool UpdateCurrentEntryElapsedSeconds(int nValue) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(nValue);
		}

		public static bool UpdateCurrentEntryCoverArt(byte[] pvBuffer, uint cbBufferLength) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
		}

		public static bool CurrentEntryDidChange() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_CurrentEntryDidChange();
		}

		/// <summary>
		/// <para> Queue</para>
		/// </summary>
		public static bool QueueWillChange() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_QueueWillChange();
		}

		public static bool ResetQueueEntries() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_ResetQueueEntries();
		}

		public static bool SetQueueEntry(int nID, int nPosition, string pchEntryText) {
			InteropHelp.TestIfAvailableClient();
			using (var pchEntryText2 = new InteropHelp.UTF8StringHandle(pchEntryText)) {
				return NativeMethods.ISteamMusicRemote_SetQueueEntry(nID, nPosition, pchEntryText2);
			}
		}

		public static bool SetCurrentQueueEntry(int nID) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_SetCurrentQueueEntry(nID);
		}

		public static bool QueueDidChange() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_QueueDidChange();
		}

		/// <summary>
		/// <para> Playlist</para>
		/// </summary>
		public static bool PlaylistWillChange() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_PlaylistWillChange();
		}

		public static bool ResetPlaylistEntries() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_ResetPlaylistEntries();
		}

		public static bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText) {
			InteropHelp.TestIfAvailableClient();
			using (var pchEntryText2 = new InteropHelp.UTF8StringHandle(pchEntryText)) {
				return NativeMethods.ISteamMusicRemote_SetPlaylistEntry(nID, nPosition, pchEntryText2);
			}
		}

		public static bool SetCurrentPlaylistEntry(int nID) {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_SetCurrentPlaylistEntry(nID);
		}

		public static bool PlaylistDidChange() {
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamMusicRemote_PlaylistDidChange();
		}
	}
}