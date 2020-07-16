<template>
    <div class="menuListStudent">
        <v-list-item
            v-for="(link, index) in menuListNotLoggedFilter"
            :to="link.route || ''"
            :href="link.url || ''"
            :key="index"
            class="link"
            color="#fff"
            sel="menu_row"
            :target="link.url ? '_blank' : ''"
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
        menuListNotLogged: {
            required: true
        },
        menuListStudent: {
            required: false
        },
        menuListTeacher: {
            required: false
        }
    },
    computed: {
        menuListNotLoggedFilter() {
            return this.menuListNotLogged.filter(item => {
                let isShowFindTutor = item.route && this.$route.name === item.route.name
                let isShowTeachOnSpitball = item.notShowFrymo && this.isFrymo 
                return !isShowFindTutor && !isShowTeachOnSpitball
            })
        },
        isFrymo() {
            return this.$store.getters.isFrymo
        }
    }
}
</script>