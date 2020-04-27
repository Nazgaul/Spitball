<template>
   <router-link to="/" sel="sidemenu_home">
      <v-list-group active-class="''" :prepend-icon="'sbf-home-sideMenu'" :append-icon="''" no-action class="sideMenu_group">
         <template v-slot:activator>
            <v-list-item class="sideMenu_list">
               <v-list-item-content>
               <v-list-item-title>
                  <span class="sideMenu_list_title" v-text="$t('schoolBlock_home')"/>
                  </v-list-item-title>
               </v-list-item-content>
            </v-list-item>
         </template>
         </v-list-group>
   </router-link>
</template>
