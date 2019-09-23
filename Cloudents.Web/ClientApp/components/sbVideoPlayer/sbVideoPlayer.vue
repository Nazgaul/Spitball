<template>
      <video v-if="src"
             :id="id" 
             :poster="poster || posterPlaceHolder" 
             :tabindex="tabindex" 
              class="azuremediaplayer amp-default-skin amp-big-play-centered">
    </video>
</template>

<script src="./sbVideoPlayer.js"/>
<style src="./sbVideoPlayer.less"/>