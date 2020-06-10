<template>
    <div class="menuListTeacher">
        <v-list-item
            v-for="(link, index) in menuListTeacher"
            :href="link.url || ''"
            @click="link.action ? link.action() :  ''"
            :key="index"
            sel="menu_row"
            link
            target="_blank"
        >
            <v-list-item-action>
                <v-icon class="userMenu_icons" size="18" color="#69687d">{{link.icon}}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
                <v-list-item-title class="userMenu_titles" v-t="link.title"></v-list-item-title>
            </v-list-item-content>
        </v-list-item>
    </div>
</template>

<script>
export default {
    props: {
        menuListTeacher: {
            required: true
        },
       menuListStudent: {
            required: false
        },
        menuListNotLogged: {
            required: false
        }
    }
}
</script>