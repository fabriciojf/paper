<template lang="pug">
  v-flex(
    xs12
    right
    v-if="!$paper.state.selection"
  )
    v-btn(
      color="secondary"
      flat
      :icon="$vuetify.breakpoint.xs"
      :disabled="!$paper.pagination.showFirst()"
      @click.stop="$paper.pagination.goToFirstPage()"
    )
      v-icon first_page
      span(class="hidden-xs-only") Primeira Página

    v-btn(
      color="secondary"
      flat
      :icon="$vuetify.breakpoint.xs"
      :disabled="!$paper.pagination.showPrevious()"
      @click.stop="$paper.pagination.goToPreviousPage()"
    )
      v-icon navigate_before
      span(class="hidden-xs-only") Anterior
      
    v-btn(
      color="secondary"
      flat
      :icon="$vuetify.breakpoint.xs"
      :disabled="!$paper.pagination.showNext()"
      @click.stop="$paper.pagination.goToNextPage()"
    )
      v-icon navigate_next
      span(class="hidden-xs-only") Próxima
</template>
