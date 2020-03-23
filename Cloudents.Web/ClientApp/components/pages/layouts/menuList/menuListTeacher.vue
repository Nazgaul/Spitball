<template>
    <div>
        <v-list-item v-for="link in satelliteLinksTeacher" :key="link.title" sel="menu_row" link>
            <v-list-item-action>
                <a :href="link.url"><v-icon class="userMenu_icons">{{link.icon}}</v-icon></a>
            </v-list-item-action>
            <v-list-item-content>
                <a :href="link.url" class="v-list__tile__title subheading userMenu_titles">{{link.title}}</a>
            </v-list-item-content>
        </v-list-item>
    </div>
</template>

<script>
export default {
    props: {
        satelliteLinksTeacher: {
            required: true
        },
        satelliteLinksStudent: {
            required: false
        }
    }
}
</script>