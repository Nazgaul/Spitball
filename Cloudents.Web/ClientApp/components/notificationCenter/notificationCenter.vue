<template>
    <div class="notification-center-wrap">
        <ask-question-btn></ask-question-btn>
        <div class="notification-center">
        <v-list three-line class="notifications-list">
            <v-subheader  class="notification-center-header" :key="'header'" >
                Notifications
            </v-subheader>
            <div v-for="(item, index) in notificationItems">
                <v-list-tile
                        class="singleNotification"
                        :key="item.title"
                        @click="doNotificationAction(item)"
                        avatar
                        ripple>
                    <v-list-tile-avatar class="notification-action-type">
                        <v-icon class="notification-action-type-icon" :color="item.active ? 'teal' : 'grey'">{{item.icon}}</v-icon>
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                        <v-list-tile-title :class="[item.isVisited ? 'visited': '', 'notification-title']">{{ item.title }}</v-list-tile-title>
                        <v-list-tile-sub-title  :class="[item.isVisited ? 'visited': '', 'notification-subtitle']">{{ item.headline }}</v-list-tile-sub-title>
                        <v-list-tile-sub-title class="timeago">
                            <span class="timeago" :datetime="item.timeago || item.create"></span>
                           </v-list-tile-sub-title>
                    </v-list-tile-content>
                    <v-list-tile-action class="notification-archive"  @click="archiveNotification(item)">
                        <v-icon class="notification-archive-icon"

                                :color="item.isVisited ? 'teal' : 'grey'">sbf-archive</v-icon>
                    </v-list-tile-action>

                </v-list-tile>
            </div>
        </v-list>
        </div>

    </div>
</template>

<script src="./notificationCenter.js"></script>

<style src="./notificationCenter.less" lang="less"></style>