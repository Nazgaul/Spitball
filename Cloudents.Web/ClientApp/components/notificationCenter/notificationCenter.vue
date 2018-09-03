<template>
    <div>
        <ask-question-btn></ask-question-btn>
        <v-list three-line>
            <div v-for="(item, index) in items">
                <v-list-tile
                        :key="item.title"
                        avatar
                        ripple>
                    <v-list-tile-avatar>
                        <v-icon :color="item.active ? 'teal' : 'grey'">sbf-help</v-icon>
                    </v-list-tile-avatar>

                    <v-list-tile-content>
                        <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                        <v-list-tile-sub-title class="text--primary">{{ item.headline }}</v-list-tile-sub-title>
                        <v-list-tile-sub-title>{{ item.subtitle }}</v-list-tile-sub-title>

                    </v-list-tile-content>
                    <v-list-tile-action>
                        <v-icon :color="item.active ? 'teal' : 'grey'">sbf-help</v-icon>
                    </v-list-tile-action>

                </v-list-tile>
                
            </div>
        </v-list>

    </div>
</template>

<script src="./notificationCenter.js"></script>

<style src="./notificationCenter.less" lang="less"></style>